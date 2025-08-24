import React, { useState, useCallback, useMemo } from 'react';
import {
  Box,
  Paper,
  Typography,
  Grid,
  Card,
  CardContent,
  FormControl,
  InputLabel,
  Select,
  MenuItem,
  Button,
  Chip,
  useTheme,
  LinearProgress,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  IconButton,
  Tooltip,
} from '@mui/material';
import {
  TrendingUp as TrendingUpIcon,
  TrendingDown as TrendingDownIcon,
  BarChart as BarChartIcon,
  PieChart as PieChartIcon,
  Timeline as TimelineIcon,
  Download as DownloadIcon,
  Refresh as RefreshIcon,
  Visibility as VisibilityIcon,
  School as SchoolIcon,
  QuestionAnswer as QuestionAnswerIcon,
  Timer as TimerIcon,
  Star as StarIcon,
} from '@mui/icons-material';

import { 
  QuestionAnalytics, 
  TagAnalytics, 
  CategoryPerformance, 
  PerformanceTrend,
  QuestionBank 
} from '../types';

interface QuestionAnalyticsDashboardProps {
  analytics: QuestionAnalytics;
  questionBanks: QuestionBank[];
  onRefresh?: () => void;
  onExport?: (format: string) => void;
  onDateRangeChange?: (range: string) => void;
  loading?: boolean;
  error?: string | null;
}

const QuestionAnalyticsDashboard: React.FC<QuestionAnalyticsDashboardProps> = ({
  analytics,
  questionBanks,
  onRefresh,
  onExport,
  onDateRangeChange,
  loading = false,
  error = null,
}) => {
  const theme = useTheme();
  const [selectedQuestionBank, setSelectedQuestionBank] = useState<number>(0);
  const [selectedDateRange, setSelectedDateRange] = useState<string>('30d');

  // Date range options
  const dateRanges = [
    { value: '7d', label: 'Last 7 Days' },
    { value: '30d', label: 'Last 30 Days' },
    { value: '90d', label: 'Last 90 Days' },
    { value: '1y', label: 'Last Year' },
    { value: 'all', label: 'All Time' },
  ];

  // Mock analytics data (would come from API)
  const mockAnalytics: QuestionAnalytics = {
    totalQuestions: 1250,
    totalQuestionBanks: 15,
    averageDifficulty: 3.2,
    averageTimeLimit: 120,
    totalPoints: 12500,
    questionsByType: {
      MultipleChoice: 450,
      TrueFalse: 200,
      Essay: 150,
      ShortAnswer: 200,
      Matching: 100,
      FillInTheBlank: 150,
    },
    questionsByDifficulty: {
      Easy: 300,
      Medium: 500,
      Hard: 450,
    },
    questionsByStatus: {
      Draft: 100,
      Active: 1000,
      Inactive: 100,
      Archived: 50,
    },
    questionsByCategory: {
      Mathematics: 400,
      Science: 350,
      History: 200,
      Language: 200,
      Geography: 100,
    },
    recentActivity: {
      questionsCreated: 25,
      questionsUpdated: 15,
      questionsDeleted: 5,
      newQuestionBanks: 2,
    },
    performanceMetrics: {
      averageResponseTime: 45,
      successRate: 78.5,
      difficultyDistribution: [25, 40, 35],
      timeDistribution: [30, 45, 25],
    },
    topTags: [
      { name: 'Algebra', count: 150, usage: 85 },
      { name: 'Physics', count: 120, usage: 78 },
      { name: 'World War II', count: 80, usage: 65 },
      { name: 'Grammar', count: 75, usage: 60 },
      { name: 'Chemistry', count: 70, usage: 55 },
    ],
    categoryPerformance: [
      { name: 'Mathematics', avgScore: 82, totalQuestions: 400, difficulty: 3.5 },
      { name: 'Science', avgScore: 78, totalQuestions: 350, difficulty: 3.2 },
      { name: 'History', avgScore: 75, totalQuestions: 200, difficulty: 2.8 },
      { name: 'Language', avgScore: 80, totalQuestions: 200, difficulty: 3.0 },
      { name: 'Geography', avgScore: 72, totalQuestions: 100, difficulty: 2.5 },
    ],
    trends: [
      { month: 'Jan', questions: 120, banks: 2, performance: 75 },
      { month: 'Feb', questions: 135, banks: 3, performance: 78 },
      { month: 'Mar', questions: 150, banks: 4, performance: 80 },
      { month: 'Apr', questions: 140, banks: 3, performance: 82 },
      { month: 'May', questions: 160, banks: 5, performance: 85 },
      { month: 'Jun', questions: 175, banks: 6, performance: 88 },
    ],
  };

  const currentAnalytics = analytics || mockAnalytics;

  // Computed values
  const totalQuestions = currentAnalytics.totalQuestions;
  const totalBanks = currentAnalytics.totalQuestionBanks;
  const avgDifficulty = currentAnalytics.averageDifficulty;
  const avgTimeLimit = currentAnalytics.averageTimeLimit;
  const totalPoints = currentAnalytics.totalPoints;

  const recentActivity = currentAnalytics.recentActivity;
  const performanceMetrics = currentAnalytics.performanceMetrics;

  // Event handlers
  const handleQuestionBankChange = useCallback((bankId: number) => {
    setSelectedQuestionBank(bankId);
  }, []);

  const handleDateRangeChange = useCallback((range: string) => {
    setSelectedDateRange(range);
    onDateRangeChange?.(range);
  }, [onDateRangeChange]);

  const handleRefresh = useCallback(() => {
    onRefresh?.();
  }, [onRefresh]);

  const handleExport = useCallback((format: string) => {
    onExport?.(format);
  }, [onExport]);

  // Render functions
  const renderOverviewCards = () => (
    <Grid container spacing={3} sx={{ mb: 4 }}>
      <Grid item xs={12} sm={6} md={3}>
        <Card sx={{ bgcolor: 'primary.light', color: 'white' }}>
          <CardContent>
            <Box sx={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between' }}>
              <Box>
                <Typography variant="h4" sx={{ fontWeight: 600, mb: 1 }}>
                  {totalQuestions.toLocaleString()}
                </Typography>
                <Typography variant="body2">Total Questions</Typography>
              </Box>
              <QuestionAnswerIcon sx={{ fontSize: 40, opacity: 0.8 }} />
            </Box>
          </CardContent>
        </Card>
      </Grid>

      <Grid item xs={12} sm={6} md={3}>
        <Card sx={{ bgcolor: 'success.light', color: 'white' }}>
          <CardContent>
            <Box sx={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between' }}>
              <Box>
                <Typography variant="h4" sx={{ fontWeight: 600, mb: 1 }}>
                  {totalBanks}
                </Typography>
                <Typography variant="body2">Question Banks</Typography>
              </Box>
              <SchoolIcon sx={{ fontSize: 40, opacity: 0.8 }} />
            </Box>
          </CardContent>
        </Card>
      </Grid>

      <Grid item xs={12} sm={6} md={3}>
        <Card sx={{ bgcolor: 'info.light', color: 'white' }}>
          <CardContent>
            <Box sx={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between' }}>
              <Box>
                <Typography variant="h4" sx={{ fontWeight: 600, mb: 1 }}>
                  {avgDifficulty.toFixed(1)}
                </Typography>
                <Typography variant="body2">Avg Difficulty</Typography>
              </Box>
              <StarIcon sx={{ fontSize: 40, opacity: 0.8 }} />
            </Box>
          </CardContent>
        </Card>
      </Grid>

      <Grid item xs={12} sm={6} md={3}>
        <Card sx={{ bgcolor: 'warning.light', color: 'white' }}>
          <CardContent>
            <Box sx={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between' }}>
              <Box>
                <Typography variant="h4" sx={{ fontWeight: 600, mb: 1 }}>
                  {totalPoints.toLocaleString()}
                </Typography>
                <Typography variant="body2">Total Points</Typography>
              </Box>
              <TrendingUpIcon sx={{ fontSize: 40, opacity: 0.8 }} />
            </Box>
          </CardContent>
        </Card>
      </Grid>
    </Grid>
  );

  const renderFilters = () => (
    <Paper sx={{ p: 3, mb: 4 }}>
      <Box sx={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between', mb: 3 }}>
        <Typography variant="h6">Analytics Filters</Typography>
        <Box sx={{ display: 'flex', gap: 2 }}>
          <Button
            variant="outlined"
            startIcon={<RefreshIcon />}
            onClick={handleRefresh}
            disabled={loading}
          >
            Refresh
          </Button>
          <Button
            variant="outlined"
            startIcon={<DownloadIcon />}
            onClick={() => handleExport('excel')}
            disabled={loading}
          >
            Export
          </Button>
        </Box>
      </Box>

      <Grid container spacing={3}>
        <Grid item xs={12} md={6}>
          <FormControl fullWidth>
            <InputLabel>Question Bank</InputLabel>
            <Select
              value={selectedQuestionBank}
              onChange={(e) => handleQuestionBankChange(e.target.value as number)}
              label="Question Bank"
              disabled={loading}
            >
              <MenuItem value={0}>All Question Banks</MenuItem>
              {questionBanks.map((bank) => (
                <MenuItem key={bank.id} value={bank.id}>
                  {bank.name}
                </MenuItem>
              ))}
            </Select>
          </FormControl>
        </Grid>

        <Grid item xs={12} md={6}>
          <FormControl fullWidth>
            <InputLabel>Date Range</InputLabel>
            <Select
              value={selectedDateRange}
              onChange={(e) => handleDateRangeChange(e.target.value)}
              label="Date Range"
              disabled={loading}
            >
              {dateRanges.map((range) => (
                <MenuItem key={range.value} value={range.value}>
                  {range.label}
                </MenuItem>
              ))}
            </Select>
          </FormControl>
        </Grid>
      </Grid>
    </Paper>
  );

  const renderPerformanceMetrics = () => (
    <Grid container spacing={3} sx={{ mb: 4 }}>
      <Grid item xs={12} md={6}>
        <Paper sx={{ p: 3 }}>
          <Typography variant="h6" sx={{ mb: 2, display: 'flex', alignItems: 'center', gap: 1 }}>
            <BarChartIcon />
            Question Distribution by Type
          </Typography>
          <Box>
            {Object.entries(currentAnalytics.questionsByType).map(([type, count]) => (
              <Box key={type} sx={{ mb: 2 }}>
                <Box sx={{ display: 'flex', justifyContent: 'space-between', mb: 1 }}>
                  <Typography variant="body2">{type}</Typography>
                  <Typography variant="body2" color="primary">
                    {count} ({((count / totalQuestions) * 100).toFixed(1)}%)
                  </Typography>
                </Box>
                <LinearProgress
                  variant="determinate"
                  value={(count / totalQuestions) * 100}
                  sx={{ height: 8, borderRadius: 4 }}
                />
              </Box>
            ))}
          </Box>
        </Paper>
      </Grid>

      <Grid item xs={12} md={6}>
        <Paper sx={{ p: 3 }}>
          <Typography variant="h6" sx={{ mb: 2, display: 'flex', alignItems: 'center', gap: 1 }}>
            <PieChartIcon />
            Question Distribution by Difficulty
          </Typography>
          <Box>
            {Object.entries(currentAnalytics.questionsByDifficulty).map(([difficulty, count]) => (
              <Box key={difficulty} sx={{ mb: 2 }}>
                <Box sx={{ display: 'flex', justifyContent: 'space-between', mb: 1 }}>
                  <Typography variant="body2">{difficulty}</Typography>
                  <Typography variant="body2" color="primary">
                    {count} ({((count / totalQuestions) * 100).toFixed(1)}%)
                  </Typography>
                </Box>
                <LinearProgress
                  variant="determinate"
                  value={(count / totalQuestions) * 100}
                  sx={{ 
                    height: 8, 
                    borderRadius: 4,
                    bgcolor: difficulty === 'Easy' ? 'success.light' : 
                             difficulty === 'Medium' ? 'warning.light' : 'error.light'
                  }}
                />
              </Box>
            ))}
          </Box>
        </Paper>
      </Grid>
    </Grid>
  );

  const renderRecentActivity = () => (
    <Paper sx={{ p: 3, mb: 4 }}>
      <Typography variant="h6" sx={{ mb: 2, display: 'flex', alignItems: 'center', gap: 1 }}>
        <TimelineIcon />
        Recent Activity
      </Typography>
      
      <Grid container spacing={3}>
        <Grid item xs={12} sm={6} md={3}>
          <Box sx={{ textAlign: 'center', p: 2, bgcolor: 'success.50', borderRadius: 2 }}>
            <Typography variant="h4" color="success.main" sx={{ fontWeight: 600 }}>
              {recentActivity.questionsCreated}
            </Typography>
            <Typography variant="body2" color="text.secondary">
              Questions Created
            </Typography>
          </Box>
        </Grid>
        
        <Grid item xs={12} sm={6} md={3}>
          <Box sx={{ textAlign: 'center', p: 2, bgcolor: 'info.50', borderRadius: 2 }}>
            <Typography variant="h4" color="info.main" sx={{ fontWeight: 600 }}>
              {recentActivity.questionsUpdated}
            </Typography>
            <Typography variant="body2" color="text.secondary">
              Questions Updated
            </Typography>
          </Box>
        </Grid>
        
        <Grid item xs={12} sm={6} md={3}>
          <Box sx={{ textAlign: 'center', p: 2, bgcolor: 'warning.50', borderRadius: 2 }}>
            <Typography variant="h4" color="warning.main" sx={{ fontWeight: 600 }}>
              {recentActivity.questionsDeleted}
            </Typography>
            <Typography variant="body2" color="text.secondary">
              Questions Deleted
            </Typography>
          </Box>
        </Grid>
        
        <Grid item xs={12} sm={6} md={3}>
          <Box sx={{ textAlign: 'center', p: 2, bgcolor: 'primary.50', borderRadius: 2 }}>
            <Typography variant="h4" color="primary.main" sx={{ fontWeight: 600 }}>
              {recentActivity.newQuestionBanks}
            </Typography>
            <Typography variant="body2" color="text.secondary">
              New Banks
            </Typography>
          </Box>
        </Grid>
      </Grid>
    </Paper>
  );

  const renderTopTags = () => (
    <Paper sx={{ p: 3, mb: 4 }}>
      <Typography variant="h6" sx={{ mb: 2 }}>
        Top Tags
      </Typography>
      
      <Box sx={{ display: 'flex', flexWrap: 'wrap', gap: 1 }}>
        {currentAnalytics.topTags.map((tag) => (
          <Chip
            key={tag.name}
            label={`${tag.name} (${tag.count})`}
            variant="outlined"
            color="primary"
            sx={{ 
              fontSize: '0.875rem',
              '& .MuiChip-label': { px: 2 }
            }}
          />
        ))}
      </Box>
    </Paper>
  );

  const renderCategoryPerformance = () => (
    <Paper sx={{ p: 3, mb: 4 }}>
      <Typography variant="h6" sx={{ mb: 2 }}>
        Category Performance
      </Typography>
      
      <TableContainer>
        <Table>
          <TableHead>
            <TableRow>
              <TableCell>Category</TableCell>
              <TableCell align="right">Questions</TableCell>
              <TableCell align="right">Avg Score</TableCell>
              <TableCell align="right">Difficulty</TableCell>
              <TableCell align="right">Actions</TableCell>
            </TableRow>
          </TableHead>
          <TableBody>
            {currentAnalytics.categoryPerformance.map((category) => (
              <TableRow key={category.name}>
                <TableCell>{category.name}</TableCell>
                <TableCell align="right">{category.totalQuestions}</TableCell>
                <TableCell align="right">
                  <Box sx={{ display: 'flex', alignItems: 'center', justifyContent: 'flex-end' }}>
                    <Typography variant="body2" sx={{ mr: 1 }}>
                      {category.avgScore}%
                    </Typography>
                    <LinearProgress
                      variant="determinate"
                      value={category.avgScore}
                      sx={{ width: 60, height: 6, borderRadius: 3 }}
                    />
                  </Box>
                </TableCell>
                <TableCell align="right">
                  <Chip
                    label={category.difficulty.toFixed(1)}
                    size="small"
                    color={
                      category.difficulty <= 2.5 ? 'success' :
                      category.difficulty <= 3.5 ? 'warning' : 'error'
                    }
                  />
                </TableCell>
                <TableCell align="right">
                  <Tooltip title="View Details">
                    <IconButton size="small">
                      <VisibilityIcon />
                    </IconButton>
                  </Tooltip>
                </TableCell>
              </TableRow>
            ))}
          </TableBody>
        </Table>
      </TableContainer>
    </Paper>
  );

  const renderTrends = () => (
    <Paper sx={{ p: 3 }}>
      <Typography variant="h6" sx={{ mb: 2 }}>
        Performance Trends
      </Typography>
      
      <Grid container spacing={2}>
        {currentAnalytics.trends.map((trend) => (
          <Grid item xs={12} sm={6} md={4} lg={2} key={trend.month}>
            <Box sx={{ textAlign: 'center', p: 2, bgcolor: 'background.default', borderRadius: 2 }}>
              <Typography variant="h6" color="primary.main">
                {trend.month}
              </Typography>
              <Typography variant="body2" color="text.secondary">
                {trend.questions} questions
              </Typography>
              <Typography variant="body2" color="text.secondary">
                {trend.banks} banks
              </Typography>
              <Typography variant="body2" color="success.main" sx={{ fontWeight: 600 }}>
                {trend.performance}% performance
              </Typography>
            </Box>
          </Grid>
        ))}
      </Grid>
    </Paper>
  );

  if (loading) {
    return (
      <Box>
        <LinearProgress sx={{ mb: 2 }} />
        <Typography>Loading analytics...</Typography>
      </Box>
    );
  }

  if (error) {
    return (
      <Alert severity="error">
        {error}
      </Alert>
    );
  }

  return (
    <Box>
      {/* Header */}
      <Box sx={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between', mb: 3 }}>
        <Box>
          <Typography variant="h4" sx={{ fontWeight: 600, mb: 1 }}>
            Question Analytics Dashboard
          </Typography>
          <Typography variant="body1" color="text.secondary">
            Comprehensive insights into your question bank performance
          </Typography>
        </Box>
      </Box>

      {renderOverviewCards()}
      {renderFilters()}
      {renderPerformanceMetrics()}
      {renderRecentActivity()}
      {renderTopTags()}
      {renderCategoryPerformance()}
      {renderTrends()}
    </Box>
  );
};

export default QuestionAnalyticsDashboard;
