// Question Bank Tree Service
// Handles all API calls for tree management, categories, and hierarchy operations

import { 
  QuestionBankTreeDto, 
  QuestionBankCategoryDto,
  CreateQuestionBankCategoryDto,
  UpdateQuestionBankCategoryDto,
  MoveCategoryDto,
  CopyCategoryDto,
  ReorderCategoriesDto,
  CategorySearchDto,
  CategoryFilterDto,
  TreeValidationResultDto,
  TreeStatisticsDto,
  PagedResult,
  ApiResponse
} from '../types/questionBankTree.types';
import { API_ENDPOINTS } from '../constants/treeOperations';

class QuestionBankTreeService {
  private baseUrl: string;

  constructor() {
    this.baseUrl = import.meta.env.VITE_API_BASE_URL || 'http://localhost:5000';
  }

  private async makeRequest<T>(
    endpoint: string, 
    options: RequestInit = {}
  ): Promise<T> {
    try {
      const token = localStorage.getItem('accessToken');
      const headers: HeadersInit = {
        'Content-Type': 'application/json',
        ...(token && { Authorization: `Bearer ${token}` }),
        ...options.headers
      };

      const response = await fetch(`${this.baseUrl}${endpoint}`, {
        ...options,
        headers
      });

      if (!response.ok) {
        const errorData = await response.json().catch(() => ({}));
        throw new Error(errorData.message || `HTTP error! status: ${response.status}`);
      }

      const data = await response.json();
      return data;
    } catch (error) {
      console.error('API request failed:', error);
      throw error;
    }
  }

  // Tree Structure Operations
  async getCompleteTree(rootCategoryId?: number, maxDepth: number = 10): Promise<QuestionBankTreeDto> {
    const params = new URLSearchParams();
    if (rootCategoryId) params.append('rootCategoryId', rootCategoryId.toString());
    params.append('maxDepth', maxDepth.toString());

    return this.makeRequest<QuestionBankTreeDto>(`${API_ENDPOINTS.TREE}?${params}`);
  }

  async getTreeByRoot(rootId: number, maxDepth: number = 10): Promise<QuestionBankTreeDto> {
    return this.makeRequest<QuestionBankTreeDto>(`${API_ENDPOINTS.TREE}?rootCategoryId=${rootId}&maxDepth=${maxDepth}`);
  }

  // Category Operations
  async getCategory(categoryId: number): Promise<QuestionBankCategoryDto> {
    return this.makeRequest<QuestionBankCategoryDto>(`${API_ENDPOINTS.CATEGORY.replace('{id}', categoryId.toString())}`);
  }

  async createCategory(dto: CreateQuestionBankCategoryDto): Promise<QuestionBankCategoryDto> {
    return this.makeRequest<QuestionBankCategoryDto>(API_ENDPOINTS.CATEGORIES, {
      method: 'POST',
      body: JSON.stringify(dto)
    });
  }

  async updateCategory(categoryId: number, dto: UpdateQuestionBankCategoryDto): Promise<QuestionBankCategoryDto> {
    return this.makeRequest<QuestionBankCategoryDto>(`${API_ENDPOINTS.CATEGORY.replace('{id}', categoryId.toString())}`, {
      method: 'PUT',
      body: JSON.stringify(dto)
    });
  }

  async deleteCategory(categoryId: number, strategy: number = 1): Promise<boolean> {
    return this.makeRequest<boolean>(`${API_ENDPOINTS.CATEGORY.replace('{id}', categoryId.toString())}?strategy=${strategy}`, {
      method: 'DELETE'
    });
  }

  // Tree Navigation Operations
  async getChildren(parentId: number): Promise<QuestionBankCategoryDto[]> {
    return this.makeRequest<QuestionBankCategoryDto[]>(`${API_ENDPOINTS.CHILDREN.replace('{id}', parentId.toString())}`);
  }

  async getParent(categoryId: number): Promise<QuestionBankCategoryDto | null> {
    try {
      return await this.makeRequest<QuestionBankCategoryDto>(`${API_ENDPOINTS.PARENT.replace('{id}', categoryId.toString())}`);
    } catch (error) {
      if (error instanceof Error && error.message.includes('not found')) {
        return null;
      }
      throw error;
    }
  }

  async getAncestors(categoryId: number): Promise<QuestionBankCategoryDto[]> {
    return this.makeRequest<QuestionBankCategoryDto[]>(`${API_ENDPOINTS.ANCESTORS.replace('{id}', categoryId.toString())}`);
  }

  async getDescendants(categoryId: number, maxDepth: number = 10): Promise<QuestionBankCategoryDto[]> {
    return this.makeRequest<QuestionBankCategoryDto[]>(`${API_ENDPOINTS.DESCENDANTS.replace('{id}', categoryId.toString())}?maxDepth=${maxDepth}`);
  }

  async getSiblings(categoryId: number): Promise<QuestionBankCategoryDto[]> {
    return this.makeRequest<QuestionBankCategoryDto[]>(`${API_ENDPOINTS.SIBLINGS.replace('{id}', categoryId.toString())}`);
  }

  // Tree Manipulation Operations
  async moveCategory(categoryId: number, dto: MoveCategoryDto): Promise<boolean> {
    return this.makeRequest<boolean>(`${API_ENDPOINTS.MOVE.replace('{id}', categoryId.toString())}`, {
      method: 'PUT',
      body: JSON.stringify(dto)
    });
  }

  async copyCategory(categoryId: number, dto: CopyCategoryDto): Promise<QuestionBankCategoryDto> {
    return this.makeRequest<QuestionBankCategoryDto>(`${API_ENDPOINTS.COPY.replace('{id}', categoryId.toString())}`, {
      method: 'POST',
      body: JSON.stringify(dto)
    });
  }

  async reorderCategories(dto: ReorderCategoriesDto): Promise<boolean> {
    return this.makeRequest<boolean>(API_ENDPOINTS.REORDER, {
      method: 'PUT',
      body: JSON.stringify(dto)
    });
  }

  // Search and Filter Operations
  async searchCategories(searchDto: CategorySearchDto): Promise<QuestionBankCategoryDto[]> {
    const params = new URLSearchParams();
    params.append('searchTerm', searchDto.query);
    if (searchDto.parentId) params.append('parentId', searchDto.parentId.toString());
    if (searchDto.includeInactive) params.append('includeInactive', searchDto.includeInactive.toString());
    if (searchDto.maxResults) params.append('maxResults', searchDto.maxResults.toString());
    if (searchDto.includeQuestions) params.append('includeQuestions', searchDto.includeQuestions.toString());

    return this.makeRequest<QuestionBankCategoryDto[]>(`${API_ENDPOINTS.SEARCH}?${params}`);
  }

  async getCategories(filter: CategoryFilterDto): Promise<PagedResult<QuestionBankCategoryDto>> {
    const params = new URLSearchParams();
    if (filter.parentId) params.append('parentId', filter.parentId.toString());
    if (filter.isActive !== undefined) params.append('isActive', filter.isActive.toString());
    if (filter.treeLevel) params.append('treeLevel', filter.treeLevel.toString());
    if (filter.hasQuestions !== undefined) params.append('hasQuestions', filter.hasQuestions.toString());
    if (filter.createdAfter) params.append('createdAfter', filter.createdAfter);
    if (filter.createdBefore) params.append('createdBefore', filter.createdBefore);
    if (filter.modifiedAfter) params.append('modifiedAfter', filter.modifiedAfter);
    if (filter.modifiedBefore) params.append('modifiedBefore', filter.modifiedBefore);
    if (filter.page) params.append('page', filter.page.toString());
    if (filter.pageSize) params.append('pageSize', filter.pageSize.toString());

    return this.makeRequest<PagedResult<QuestionBankCategoryDto>>(`${API_ENDPOINTS.CATEGORIES}?${params}`);
  }

  async findCategoryByPath(treePath: string): Promise<QuestionBankCategoryDto | null> {
    try {
      return await this.makeRequest<QuestionBankCategoryDto>(`${API_ENDPOINTS.FIND_BY_PATH}?treePath=${encodeURIComponent(treePath)}`);
    } catch (error) {
      if (error instanceof Error && error.message.includes('not found')) {
        return null;
      }
      throw error;
    }
  }

  // Tree Validation Operations
  async validateTreeStructure(): Promise<TreeValidationResultDto> {
    return this.makeRequest<TreeValidationResultDto>(API_ENDPOINTS.VALIDATE_STRUCTURE);
  }

  async validateCategoryHierarchy(categoryId: number): Promise<boolean> {
    return this.makeRequest<boolean>(`${API_ENDPOINTS.VALIDATE_HIERARCHY.replace('{id}', categoryId.toString())}`);
  }

  // Tree Statistics
  async getTreeStatistics(): Promise<TreeStatisticsDto> {
    return this.makeRequest<TreeStatisticsDto>(API_ENDPOINTS.STATISTICS);
  }

  // Bulk Operations
  async bulkCreateCategories(categories: CreateQuestionBankCategoryDto[]): Promise<boolean> {
    return this.makeRequest<boolean>(API_ENDPOINTS.BULK_CREATE, {
      method: 'POST',
      body: JSON.stringify(categories)
    });
  }

  async getRecentlyModified(sinceDate: Date, maxResults: number = 100): Promise<QuestionBankCategoryDto[]> {
    const params = new URLSearchParams();
    params.append('sinceDate', sinceDate.toISOString());
    params.append('maxResults', maxResults.toString());

    return this.makeRequest<QuestionBankCategoryDto[]>(`${API_ENDPOINTS.RECENTLY_MODIFIED}?${params}`);
  }

  // Utility Methods
  async getCategoryPath(categoryId: number): Promise<QuestionBankCategoryDto[]> {
    try {
      const ancestors = await this.getAncestors(categoryId);
      const current = await this.getCategory(categoryId);
      return [...ancestors, current];
    } catch (error) {
      console.error('Error getting category path:', error);
      return [];
    }
  }

  async getCategoryWithChildren(categoryId: number, maxDepth: number = 3): Promise<QuestionBankCategoryDto | null> {
    try {
      const category = await this.getCategory(categoryId);
      if (maxDepth > 0 && category.children && category.children.length > 0) {
        const children = await Promise.all(
          category.children.map(child => this.getCategoryWithChildren(child.categoryId, maxDepth - 1))
        );
        category.children = children.filter(Boolean) as QuestionBankCategoryDto[];
      }
      return category;
    } catch (error) {
      console.error('Error getting category with children:', error);
      return null;
    }
  }

  async searchCategoriesRecursive(query: string, maxResults: number = 50): Promise<QuestionBankCategoryDto[]> {
    try {
      // First, search in current level
      const directResults = await this.searchCategories({ query, maxResults });
      
      if (directResults.length >= maxResults) {
        return directResults.slice(0, maxResults);
      }

      // If we need more results, search in children recursively
      const allResults = [...directResults];
      const remainingSlots = maxResults - directResults.length;

      for (const category of directResults) {
        if (allResults.length >= maxResults) break;
        
        const children = await this.getChildren(category.categoryId);
        const childResults = await Promise.all(
          children.map(child => this.searchCategoriesRecursive(query, Math.ceil(remainingSlots / children.length)))
        );
        
        allResults.push(...childResults.flat());
      }

      return allResults.slice(0, maxResults);
    } catch (error) {
      console.error('Error in recursive search:', error);
      return [];
    }
  }

  // Cache Management
  private cache = new Map<string, { data: any; timestamp: number; ttl: number }>();

  private getCacheKey(endpoint: string, params?: Record<string, any>): string {
    const paramString = params ? JSON.stringify(params) : '';
    return `${endpoint}${paramString}`;
  }

  private isCacheValid(key: string): boolean {
    const cached = this.cache.get(key);
    if (!cached) return false;
    
    return Date.now() - cached.timestamp < cached.ttl;
  }

  private setCache(key: string, data: any, ttl: number = 5 * 60 * 1000): void {
    this.cache.set(key, { data, timestamp: Date.now(), ttl });
  }

  private getCache(key: string): any | null {
    const cached = this.cache.get(key);
    return cached && this.isCacheValid(key) ? cached.data : null;
  }

  private clearCache(pattern?: string): void {
    if (pattern) {
      for (const key of this.cache.keys()) {
        if (key.includes(pattern)) {
          this.cache.delete(key);
        }
      }
    } else {
      this.cache.clear();
    }
  }

  // Cached versions of frequently accessed methods
  async getCategoryCached(categoryId: number, ttl: number = 5 * 60 * 1000): Promise<QuestionBankCategoryDto> {
    const cacheKey = this.getCacheKey(`category-${categoryId}`);
    const cached = this.getCache(cacheKey);
    
    if (cached) {
      return cached;
    }

    const data = await this.getCategory(categoryId);
    this.setCache(cacheKey, data, ttl);
    return data;
  }

  async getChildrenCached(parentId: number, ttl: number = 2 * 60 * 1000): Promise<QuestionBankCategoryDto[]> {
    const cacheKey = this.getCacheKey(`children-${parentId}`);
    const cached = this.getCache(cacheKey);
    
    if (cached) {
      return cached;
    }

    const data = await this.getChildren(parentId);
    this.setCache(cacheKey, data, ttl);
    return data;
  }

  // Cache invalidation methods
  invalidateCategoryCache(categoryId: number): void {
    this.clearCache(`category-${categoryId}`);
    this.clearCache(`children-${categoryId}`);
    this.clearCache('tree');
  }

  invalidateTreeCache(): void {
    this.clearCache('tree');
    this.clearCache('children');
    this.clearCache('statistics');
  }

  // Error handling and retry logic
  private async retryRequest<T>(
    requestFn: () => Promise<T>, 
    maxRetries: number = 3, 
    delay: number = 1000
  ): Promise<T> {
    let lastError: Error;
    
    for (let attempt = 1; attempt <= maxRetries; attempt++) {
      try {
        return await requestFn();
      } catch (error) {
        lastError = error as Error;
        
        if (attempt === maxRetries) {
          throw lastError;
        }
        
        // Wait before retrying
        await new Promise(resolve => setTimeout(resolve, delay * attempt));
      }
    }
    
    throw lastError!;
  }

  // Batch operations for better performance
  async batchGetCategories(categoryIds: number[]): Promise<QuestionBankCategoryDto[]> {
    const batchSize = 10;
    const results: QuestionBankCategoryDto[] = [];
    
    for (let i = 0; i < categoryIds.length; i += batchSize) {
      const batch = categoryIds.slice(i, i + batchSize);
      const batchPromises = batch.map(id => this.getCategoryCached(id));
      const batchResults = await Promise.all(batchPromises);
      results.push(...batchResults);
    }
    
    return results;
  }

  async batchGetChildren(parentIds: number[]): Promise<Record<number, QuestionBankCategoryDto[]>> {
    const batchSize = 5;
    const results: Record<number, QuestionBankCategoryDto[]> = {};
    
    for (let i = 0; i < parentIds.length; i += batchSize) {
      const batch = parentIds.slice(i, i + batchSize);
      const batchPromises = batch.map(async (id) => {
        const children = await this.getChildrenCached(id);
        return { id, children };
      });
      
      const batchResults = await Promise.all(batchPromises);
      batchResults.forEach(({ id, children }) => {
        results[id] = children;
      });
    }
    
    return results;
  }
}

// Export singleton instance
export const questionBankTreeService = new QuestionBankTreeService();

// Export class for testing or custom instances
export { QuestionBankTreeService };
