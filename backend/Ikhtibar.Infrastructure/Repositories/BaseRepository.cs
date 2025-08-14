using System.Data;
using System.Reflection;
using Dapper;
using Ikhtibar.Shared.Entities;
using Ikhtibar.Core.Repositories.Interfaces;
using Ikhtibar.Infrastructure.Data;

namespace Ikhtibar.Infrastructure.Repositories;

/// <summary>
/// GUID type handler for SQLite compatibility
/// </summary>
public class GuidTypeHandler : SqlMapper.TypeHandler<Guid>
{
    public override void SetValue(IDbDataParameter parameter, Guid value)
    {
        parameter.Value = value.ToString();
        parameter.DbType = DbType.String;
    }

    public override Guid Parse(object value)
    {
        if (value == null || value is DBNull)
            return Guid.Empty;

        if (value is Guid guid)
            return guid;

        if (value is string str && !string.IsNullOrEmpty(str))
        {
            if (Guid.TryParse(str, out var result))
                return result;
        }

        if (value is byte[] bytes && bytes.Length == 16)
        {
            return new Guid(bytes);
        }

        throw new InvalidOperationException($"Cannot parse '{value}' (type: {value?.GetType()}) as Guid");
    }
}

/// <summary>
/// Nullable GUID type handler for SQLite compatibility
/// </summary>
public class NullableGuidTypeHandler : SqlMapper.TypeHandler<Guid?>
{
    public override void SetValue(IDbDataParameter parameter, Guid? value)
    {
        parameter.Value = value?.ToString() ?? (object)DBNull.Value;
        parameter.DbType = DbType.String;
    }

    public override Guid? Parse(object value)
    {
        if (value == null || value is DBNull)
            return null;

        if (value is Guid guid)
            return guid;

        if (value is string str && !string.IsNullOrEmpty(str))
        {
            if (Guid.TryParse(str, out var result))
                return result;
        }

        if (value is byte[] bytes && bytes.Length == 16)
        {
            return new Guid(bytes);
        }

        throw new InvalidOperationException($"Cannot parse '{value}' (type: {value?.GetType()}) as Guid?");
    }
}

/// <summary>
/// Base repository implementation using Dapper for data access
/// Provides common CRUD operations for all entities
/// </summary>
/// <typeparam name="T">Entity type that inherits from BaseEntity</typeparam>
public class BaseRepository<T> : IRepository<T> where T : BaseEntity
{
    protected readonly IDbConnectionFactory _connectionFactory;
    private readonly string _tableName;

    /// <summary>
    /// Static constructor to register type handlers for SQLite compatibility
    /// </summary>
    static BaseRepository()
    {
        // Remove the custom type handlers and let SQLite handle conversion
        SqlMapper.RemoveTypeMap(typeof(Guid));
        SqlMapper.RemoveTypeMap(typeof(Guid?));

        // Configure SQLite to store GUIDs as TEXT instead of BLOB
        SqlMapper.AddTypeHandler(new GuidTypeHandler());
        SqlMapper.AddTypeHandler(new NullableGuidTypeHandler());
    }

    /// <summary>
    /// Initializes a new instance of the BaseRepository
    /// </summary>
    /// <param name="connectionFactory">Factory for creating database connections</param>
    public BaseRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
        _tableName = GetTableName();
    }
    
    /// <summary>
    /// Checks if the entity type has a property with the given name
    /// </summary>
    private bool EntityHasProperty(string propertyName)
    {
        return typeof(T).GetProperty(propertyName) != null;
    }

    /// <summary>
    /// Gets the primary key column name for the entity
    /// Based on entity name convention: User -> UserId, Role -> RoleId, etc.
    /// </summary>
    private string GetPrimaryKeyColumnName()
    {
        var entityName = typeof(T).Name;
        
        // Remove "Entity" suffix if present
        if (entityName.EndsWith("Entity"))
        {
            entityName = entityName.Substring(0, entityName.Length - 6);
        }

        return $"{entityName}Id";
    }

    /// <summary>
    /// Gets an entity by its unique identifier
    /// </summary>
    /// <param name="id">The unique identifier of the entity</param>
    /// <returns>The entity if found, null otherwise</returns>
    public async Task<T?> GetByIdAsync(int id)
    {
        // Get the primary key column name for the entity
        var primaryKeyColumn = GetPrimaryKeyColumnName();
        
        var sql = $"SELECT * FROM {_tableName} WHERE {primaryKeyColumn} = @Id AND IsDeleted = 0";

        using var connection = _connectionFactory.CreateConnection();
        return await connection.QueryFirstOrDefaultAsync<T>(sql, new { Id = id });
    }

    /// <summary>
    /// Gets all entities with optional filtering
    /// </summary>
    /// <param name="where">Optional WHERE clause for filtering</param>
    /// <param name="parameters">Parameters for the WHERE clause</param>
    /// <returns>Collection of entities matching the criteria</returns>
    public async Task<IEnumerable<T>> GetAllAsync(string? where = null, object? parameters = null)
    {
        var sql = $"SELECT * FROM {_tableName} WHERE IsDeleted = 0";

        if (!string.IsNullOrEmpty(where))
        {
            sql += $" AND ({where})";
        }

        using var connection = _connectionFactory.CreateConnection();
        return await connection.QueryAsync<T>(sql, parameters);
    }

    /// <summary>
    /// Gets entities with pagination support
    /// </summary>
    /// <param name="offset">Number of records to skip</param>
    /// <param name="limit">Maximum number of records to return</param>
    /// <param name="where">Optional WHERE clause for filtering</param>
    /// <param name="orderBy">Optional ORDER BY clause</param>
    /// <param name="parameters">Parameters for the WHERE clause</param>
    /// <returns>Collection of entities matching the criteria</returns>
    public async Task<IEnumerable<T>> GetPagedAsync(int offset, int limit, string? where = null, string? orderBy = null, object? parameters = null)
    {
        var sql = $"SELECT * FROM {_tableName} WHERE IsDeleted = 0";

        if (!string.IsNullOrEmpty(where))
        {
            sql += $" AND ({where})";
        }

        if (!string.IsNullOrEmpty(orderBy))
        {
            sql += $" ORDER BY {orderBy}";
        }
        else
        {
            sql += " ORDER BY CreatedAt DESC";
        }

        // Use SQLite-compatible LIMIT and OFFSET syntax
        sql += $" LIMIT {limit} OFFSET {offset}";

        using var connection = _connectionFactory.CreateConnection();
        return await connection.QueryAsync<T>(sql, parameters);
    }

    /// <summary>
    /// Gets the total count of entities with optional filtering
    /// </summary>
    /// <param name="where">Optional WHERE clause for filtering</param>
    /// <param name="parameters">Parameters for the WHERE clause</param>
    /// <returns>Total count of entities</returns>
    public async Task<int> CountAsync(string? where = null, object? parameters = null)
    {
        var sql = $"SELECT COUNT(*) FROM {_tableName} WHERE IsDeleted = 0";

        if (!string.IsNullOrEmpty(where))
        {
            sql += $" AND ({where})";
        }

        using var connection = _connectionFactory.CreateConnection();
        return await connection.QuerySingleAsync<int>(sql, parameters);
    }

    /// <summary>
    /// Creates a new entity
    /// </summary>
    /// <param name="entity">The entity to create</param>
    /// <returns>The created entity with generated ID</returns>
    public async Task<T> AddAsync(T entity)
    {
        // Set base entity properties (ID is auto-generated by SQL Server IDENTITY)
        entity.CreatedAt = DateTime.UtcNow;
        entity.ModifiedAt = DateTime.UtcNow;
        entity.IsDeleted = false;

        var properties = GetEntityProperties(excludeId: true);
        var columns = string.Join(", ", properties.Select(p => p.Name));
        var values = string.Join(", ", properties.Select(p => "@" + p.Name));
        
        var primaryKeyColumn = GetPrimaryKeyColumnName();
        var sql = $@"
            INSERT INTO {_tableName} ({columns}) 
            VALUES ({values});
            SELECT * FROM {_tableName} WHERE {primaryKeyColumn} = SCOPE_IDENTITY();";

        using var connection = _connectionFactory.CreateConnection();
        var createdEntity = await connection.QuerySingleAsync<T>(sql, entity);
        
        return createdEntity;
    }

    /// <summary>
    /// Updates an existing entity
    /// </summary>
    /// <param name="entity">The entity to update</param>
    /// <returns>The updated entity</returns>
    public async Task<T> UpdateAsync(T entity)
    {
        // Update the ModifiedAt timestamp
        entity.ModifiedAt = DateTime.UtcNow;

        // Build SET clause from entity properties, excluding primary key and CreatedAt
        var properties = GetEntityProperties(excludeId: true, excludeCreatedAt: true);
        var setClause = string.Join(", ", properties.Select(p => $"{p.Name} = @{p.Name}"));
        
        var primaryKeyColumn = GetPrimaryKeyColumnName();
        var sql = $"UPDATE {_tableName} SET {setClause} WHERE {primaryKeyColumn} = @{primaryKeyColumn.Replace("Id", "Id")}";

        using var connection = _connectionFactory.CreateConnection();
        await connection.ExecuteAsync(sql, entity);

        return entity;
    }

    /// <summary>
    /// Performs a soft delete on the entity (sets IsDeleted = true)
    /// </summary>
    /// <param name="id">The unique identifier of the entity to delete</param>
    /// <returns>True if the entity was deleted, false if not found</returns>
    public async Task<bool> DeleteAsync(int id)
    {
        var primaryKeyColumn = GetPrimaryKeyColumnName();
        var sql = $@"
            UPDATE {_tableName} 
            SET IsDeleted = 1, DeletedAt = @DeletedAt 
            WHERE {primaryKeyColumn} = @Id AND IsDeleted = 0";

        using var connection = _connectionFactory.CreateConnection();
        var rowsAffected = await connection.ExecuteAsync(sql,
            new { Id = id, DeletedAt = DateTime.UtcNow });

        return rowsAffected > 0;
    }

    /// <summary>
    /// Permanently removes an entity from the database
    /// Use with caution - this is irreversible
    /// </summary>
    /// <param name="id">The unique identifier of the entity to permanently delete</param>
    /// <returns>True if the entity was deleted, false if not found</returns>
    public async Task<bool> HardDeleteAsync(int id)
    {
        var primaryKeyColumn = GetPrimaryKeyColumnName();
        var sql = $"DELETE FROM {_tableName} WHERE {primaryKeyColumn} = @Id";

        using var connection = _connectionFactory.CreateConnection();
        var rowsAffected = await connection.ExecuteAsync(sql, new { Id = id });

        return rowsAffected > 0;
    }

    /// <summary>
    /// Checks if an entity exists with the given ID
    /// </summary>
    /// <param name="id">The unique identifier to check</param>
    /// <returns>True if the entity exists, false otherwise</returns>
    public async Task<bool> ExistsAsync(int id)
    {
        var primaryKeyColumn = GetPrimaryKeyColumnName();
        var sql = $@"
            SELECT COUNT(1) FROM {_tableName} 
            WHERE {primaryKeyColumn} = @Id AND IsDeleted = 0";

        using var connection = _connectionFactory.CreateConnection();
        var count = await connection.QuerySingleAsync<int>(sql, new { Id = id });

        return count > 0;
    }

    /// <summary>
    /// Executes a custom SQL query and returns entities
    /// </summary>
    /// <param name="sql">The SQL query to execute</param>
    /// <param name="parameters">Parameters for the SQL query</param>
    /// <returns>Collection of entities matching the query</returns>
    public async Task<IEnumerable<T>> QueryAsync(string sql, object? parameters = null)
    {
        using var connection = _connectionFactory.CreateConnection();
        return await connection.QueryAsync<T>(sql, parameters);
    }

    /// <summary>
    /// Executes a custom SQL command (INSERT, UPDATE, DELETE)
    /// </summary>
    /// <param name="sql">The SQL command to execute</param>
    /// <param name="parameters">Parameters for the SQL command</param>
    /// <returns>Number of affected rows</returns>
    public async Task<int> ExecuteAsync(string sql, object? parameters = null)
    {
        using var connection = _connectionFactory.CreateConnection();
        return await connection.ExecuteAsync(sql, parameters);
    }

    /// <summary>
    /// Gets the table name for the entity type
    /// </summary>
    /// <returns>The table name</returns>
    private string GetTableName()
    {
        var type = typeof(T);
        var name = type.Name;

        // Remove "Entity" suffix if present and properly pluralize
        if (name.EndsWith("Entity"))
        {
            name = name[..^6]; // Remove "Entity"
        }

        // For TestEntity -> Test -> Tests
        // Simple pluralization rules
        if (name.EndsWith("y"))
        {
            return name[..^1] + "ies";
        }
        else if (name.EndsWith("s") || name.EndsWith("x") || name.EndsWith("z") ||
                 name.EndsWith("ch") || name.EndsWith("sh"))
        {
            return name + "es";
        }
        else
        {
            return name + "s";
        }
    }

    /// <summary>
    /// Gets the properties of the entity for database operations
    /// </summary>
    /// <param name="excludeId">Whether to exclude the Id property</param>
    /// <param name="excludeCreatedAt">Whether to exclude the CreatedAt property</param>
    /// <returns>Array of PropertyInfo objects</returns>
    private PropertyInfo[] GetEntityProperties(bool excludeId = false, bool excludeCreatedAt = false)
    {
        var properties = typeof(T).GetProperties()
            .Where(p => p.CanRead && p.CanWrite)
            // Exclude navigation properties and complex types
            .Where(p => IsPrimitiveOrSimpleType(p.PropertyType))
            .ToList();

        if (excludeId)
        {
            properties = properties.Where(p => p.Name != "Id").ToList();
        }

        if (excludeCreatedAt)
        {
            properties = properties.Where(p => p.Name != "CreatedAt").ToList();
        }

        return properties.ToArray();
    }

    /// <summary>
    /// Determines if a type is a primitive type or simple type that can be mapped by Dapper
    /// </summary>
    /// <param name="type">The type to check</param>
    /// <returns>True if the type can be mapped by Dapper</returns>
    private static bool IsPrimitiveOrSimpleType(Type type)
    {
        // Handle nullable types
        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
        {
            type = Nullable.GetUnderlyingType(type)!;
        }

        // Include primitive types, string, DateTime, DateTimeOffset, TimeSpan, Guid, and enums
        return type.IsPrimitive ||
               type == typeof(string) ||
               type == typeof(DateTime) ||
               type == typeof(DateTimeOffset) ||
               type == typeof(TimeSpan) ||
               type == typeof(Guid) ||
               type == typeof(decimal) ||
               type.IsEnum;
    }
}
