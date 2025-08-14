import React from 'react';
import { Box, Typography } from '@mui/material';
import { useAuditLogs } from '../hooks/useAuditLogs';
import AuditLogsList from '../components/AuditLogsList';

/**
 * AuditLogsPage component for displaying audit logs
 */
const AuditLogsPage: React.FC = () => {
  const { 
    auditLogs, 
    loading, 
    error, 
    filter, 
    setFilter, 
    refreshLogs, 
    exportLogs, 
    archiveOldLogs 
  } = useAuditLogs();

  return (
    <Box sx={{ padding: 3 }}>
      <Typography variant="h4" component="h1" gutterBottom>
        Audit Logs
      </Typography>
      <Typography variant="body2" color="text.secondary" paragraph>
        View and manage system audit logs for security and compliance purposes.
      </Typography>

      <AuditLogsList
        auditLogs={auditLogs}
        loading={loading}
        error={error}
        filter={filter}
        onFilterChange={setFilter}
        onRefresh={refreshLogs}
        onExport={exportLogs}
        onArchive={archiveOldLogs}
      />
    </Box>
  );
};

export default AuditLogsPage;
