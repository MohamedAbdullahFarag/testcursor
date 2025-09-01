import React, { useState } from 'react';
import {
  Paper,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TablePagination,
  TableRow,
  Chip,
  Card,
  CardContent,
  TextField,
  Button,
  Box,
  Grid,
  FormControl,
  InputLabel,
  Select,
  MenuItem,
  Alert
} from '@mui/material';

import { 
  AuditLog, 
  AuditLogFilter, 
  AuditSeverity, 
  AuditCategory,
  AuditLogExportFormat,
  getSeverityLabel,
  getCategoryLabel,
  AuditSeverityLabels,
  AuditCategoryLabels
} from '../types/auditLogs';

// Use a simplified PagedResult interface
interface PagedResult<T> {
  data: T[];
  total: number;
  page: number;
  pageSize: number;
}

interface AuditLogsListProps {
  auditLogs: PagedResult<AuditLog>;
  loading: boolean;
  error: string | null;
  filter: AuditLogFilter;
  onFilterChange: (filter: AuditLogFilter) => void;
  onRefresh: () => Promise<void>;
  onExport: (format: AuditLogExportFormat) => Promise<Blob>;
  onArchive: (retentionDays: number) => Promise<any>;
}

const AuditLogsList: React.FC<AuditLogsListProps> = ({
  auditLogs,
  loading,
  error,
  filter,
  onFilterChange,
  onRefresh,
  onExport,
  onArchive
}) => {
  const [showFilters, setShowFilters] = useState<boolean>(false);
  const [tempFilter, setTempFilter] = useState<Partial<AuditLogFilter>>(filter);

  // Handle page change
  const handlePageChange = (_: unknown, newPage: number) => {
    onFilterChange({ ...filter, page: newPage });
  };

  // Handle rows per page change
  const handleChangeRowsPerPage = (event: React.ChangeEvent<HTMLInputElement>) => {
    onFilterChange({
      ...filter,
      pageSize: parseInt(event.target.value, 10),
      page: 0
    });
  };

  // Text input change handler
  const handleTextInputChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    const { name, value } = event.target;
    setTempFilter({ ...tempFilter, [name]: value });
  };

  // Select input change handler
  const handleSelectChange = (name: string, value: any) => {
    setTempFilter({ ...tempFilter, [name]: value });
  };

  // Apply filters
  const applyFilters = () => {
    onFilterChange({ ...filter, ...tempFilter, page: 0 });
  };

  // Clear filters
  const clearFilters = () => {
    const clearedFilter = {
      ...filter,
      userId: undefined,
      userIdentifier: undefined,
      action: undefined,
      entityType: undefined,
      entityId: undefined,
      severity: undefined,
      category: undefined,
      fromDate: undefined,
      toDate: undefined,
      searchText: undefined,
      page: 0
    };
    setTempFilter(clearedFilter);
    onFilterChange(clearedFilter);
  };

  // Handle export
  const handleExport = async (format: AuditLogExportFormat) => {
    try {
      const blob = await onExport(format);
      // Create and trigger download
      const url = window.URL.createObjectURL(blob);
      const link = document.createElement('a');
      link.href = url;
      const timestamp = new Date().toISOString().slice(0, 10);
      link.download = `audit_logs_${format.toLowerCase()}_${timestamp}.${format === AuditLogExportFormat.Excel ? 'xlsx' : format.toLowerCase()}`;
      document.body.appendChild(link);
      link.click();
      document.body.removeChild(link);
    } catch (err) {
      console.error('Export failed:', err);
    }
  };

  // Handle archive
  const handleArchive = async () => {
    if (window.confirm('Are you sure you want to archive audit logs older than 1 year? This action cannot be undone.')) {
      try {
        await onArchive(365); // Archive logs older than 1 year
        onRefresh(); // Refresh logs after archiving
      } catch (err) {
        console.error('Archive failed:', err);
      }
    }
  };

  // Format date for display
  const formatDate = (dateString: string): string => {
    try {
      const date = new Date(dateString);
      return date.toLocaleDateString() + ' ' + date.toLocaleTimeString();
    } catch (e) {
      return 'Invalid date';
    }
  };

  // Render severity chip
  const renderSeverityChip = (severity: number) => {
    const severityLabel = getSeverityLabel(severity);
    const color = 
      severity === AuditSeverity.Critical ? 'error' :
      severity === AuditSeverity.High ? 'warning' :
      severity === AuditSeverity.Medium ? 'info' :
      'success';
    
    return <Chip label={severityLabel} color={color as any} size="small" />;
  };

  return (
    <>
      {error && <Alert severity="error" sx={{ mb: 2 }}>{error}</Alert>}

      <Card sx={{ mb: 2 }}>
        <CardContent>
          <Box display="flex" justifyContent="space-between" alignItems="center" mb={2}>
            <Button
              variant="outlined"
              onClick={() => setShowFilters(!showFilters)}
            >
              {showFilters ? 'Hide Filters' : 'Show Filters'}
            </Button>
            <Box>
              <Button
                variant="outlined"
                onClick={onRefresh}
                disabled={loading}
                sx={{ mr: 1 }}
              >
                Refresh
              </Button>
              <Button
                variant="outlined"
                onClick={() => handleExport(AuditLogExportFormat.CSV)}
                disabled={loading}
                sx={{ mr: 1 }}
              >
                Export CSV
              </Button>
              <Button
                variant="outlined"
                onClick={() => handleExport(AuditLogExportFormat.Excel)}
                disabled={loading}
                sx={{ mr: 1 }}
              >
                Export Excel
              </Button>
              <Button
                variant="outlined"
                onClick={handleArchive}
                disabled={loading}
                color="warning"
              >
                Archive Old Logs
              </Button>
            </Box>
          </Box>

          {showFilters && (
            <Box component="form" noValidate sx={{ mb: 2 }}>
              <Grid container spacing={2}>
                <Grid item xs={12} sm={6} md={4}>
                  <TextField
                    fullWidth
                    label="Search Text"
                    name="searchText"
                    value={tempFilter.searchText || ''}
                    onChange={handleTextInputChange}
                    variant="outlined"
                    size="small"
                  />
                </Grid>
                <Grid item xs={12} sm={6} md={4}>
                  <TextField
                    fullWidth
                    label="User Identifier"
                    name="userIdentifier"
                    value={tempFilter.userIdentifier || ''}
                    onChange={handleTextInputChange}
                    variant="outlined"
                    size="small"
                    placeholder="Email or username"
                  />
                </Grid>
                <Grid item xs={12} sm={6} md={4}>
                  <TextField
                    fullWidth
                    label="Action"
                    name="action"
                    value={tempFilter.action || ''}
                    onChange={handleTextInputChange}
                    variant="outlined"
                    size="small"
                  />
                </Grid>
                <Grid item xs={12} sm={6} md={4}>
                  <FormControl fullWidth size="small">
                    <InputLabel>Severity</InputLabel>
                    <Select
                      label="Severity"
                      value={tempFilter.severity ?? ''}
                      onChange={(e) => handleSelectChange('severity', e.target.value === '' ? undefined : Number(e.target.value))}
                    >
                      <MenuItem value="">Any Severity</MenuItem>
                      <MenuItem value={AuditSeverity.Critical}>Critical</MenuItem>
                      <MenuItem value={AuditSeverity.High}>High</MenuItem>
                      <MenuItem value={AuditSeverity.Medium}>Medium</MenuItem>
                      <MenuItem value={AuditSeverity.Low}>Low</MenuItem>
                    </Select>
                  </FormControl>
                </Grid>
                <Grid item xs={12} sm={6} md={4}>
                  <FormControl fullWidth size="small">
                    <InputLabel>Category</InputLabel>
                    <Select
                      label="Category"
                      value={tempFilter.category ?? ''}
                      onChange={(e) => handleSelectChange('category', e.target.value === '' ? undefined : Number(e.target.value))}
                    >
                      <MenuItem value="">Any Category</MenuItem>
                      <MenuItem value={AuditCategory.Authentication}>Authentication</MenuItem>
                      <MenuItem value={AuditCategory.Authorization}>Authorization</MenuItem>
                      <MenuItem value={AuditCategory.UserManagement}>User Management</MenuItem>
                      <MenuItem value={AuditCategory.DataAccess}>Data Access</MenuItem>
                      <MenuItem value={AuditCategory.System}>System</MenuItem>
                      <MenuItem value={AuditCategory.Security}>Security</MenuItem>
                    </Select>
                  </FormControl>
                </Grid>
                <Grid item xs={12} sm={6} md={4}>
                  <TextField
                    fullWidth
                    label="Entity Type"
                    name="entityType"
                    value={tempFilter.entityType || ''}
                    onChange={handleTextInputChange}
                    variant="outlined"
                    size="small"
                  />
                </Grid>
                <Grid item xs={12}>
                  <Box display="flex" justifyContent="flex-end" gap={1}>
                    <Button variant="text" onClick={clearFilters}>
                      Clear Filters
                    </Button>
                    <Button variant="contained" onClick={applyFilters}>
                      Apply Filters
                    </Button>
                  </Box>
                </Grid>
              </Grid>
            </Box>
          )}
        </CardContent>
      </Card>

      <TableContainer component={Paper}>
        <Table size="small">
          <TableHead>
            <TableRow>
              <TableCell>Timestamp</TableCell>
              <TableCell>User</TableCell>
              <TableCell>Action</TableCell>
              <TableCell>Entity</TableCell>
              <TableCell>Severity</TableCell>
              <TableCell>Category</TableCell>
              <TableCell>IP Address</TableCell>
            </TableRow>
          </TableHead>
          <TableBody>
            {loading ? (
              <TableRow>
                <TableCell colSpan={7} align="center">
                  Loading...
                </TableCell>
              </TableRow>
            ) : auditLogs.data.length === 0 ? (
              <TableRow>
                <TableCell colSpan={7} align="center">
                  No audit logs found matching your criteria.
                </TableCell>
              </TableRow>
            ) : (
              auditLogs.data.map((log: AuditLog) => (
                <TableRow key={log.auditLogId}>
                  <TableCell>{formatDate(log.timestamp)}</TableCell>
                  <TableCell>{log.userIdentifier || 'System'}</TableCell>
                  <TableCell>{log.action}</TableCell>
                  <TableCell>
                    {log.entityType}
                    {log.entityId && (
                      <Box component="span" sx={{ ml: 0.5, color: 'text.secondary', fontSize: '0.75rem' }}>
                        ({log.entityId})
                      </Box>
                    )}
                  </TableCell>
                  <TableCell>{renderSeverityChip(log.severity)}</TableCell>
                  <TableCell>{getCategoryLabel(log.category)}</TableCell>
                  <TableCell>{log.ipAddress || 'N/A'}</TableCell>
                </TableRow>
              ))
            )}
          </TableBody>
        </Table>
      </TableContainer>
      
      <TablePagination
        rowsPerPageOptions={[10, 25, 50, 100]}
        component="div"
        count={auditLogs.total}
        rowsPerPage={filter.pageSize}
        page={filter.page}
        onPageChange={handlePageChange}
        onRowsPerPageChange={handleChangeRowsPerPage}
        labelDisplayedRows={({ from, to, count }) => `${from}-${to} of ${count}`}
      />
    </>
  );
};

export default AuditLogsList;
