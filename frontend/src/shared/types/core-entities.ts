/**
 * Core Entity Types
 * TypeScript interfaces matching the database schema from schema.sql
 * Updated to use INT primary keys instead of GUIDs
 */

// ================= Base Entity Interface =================
export interface BaseEntity {
  id: number;
  createdAt: string;
  createdBy?: number;
  modifiedAt?: string;
  modifiedBy?: number;
  deletedAt?: string;
  deletedBy?: number;
  isDeleted: boolean;
  rowVersion?: string;
}

// ================= Lookup Tables =================
export interface QuestionType {
  questionTypeId: number;
  name: string;
}

export interface DifficultyLevel {
  difficultyLevelId: number;
  name: string;
}

export interface QuestionStatus {
  questionStatusId: number;
  name: string;
}

export interface MediaType {
  mediaTypeId: number;
  name: string;
}

export interface TreeNodeType {
  treeNodeTypeId: number;
  name: string;
}

// ================= Core Entities =================
export interface User extends BaseEntity {
  userId: number;
  username: string;
  email: string;
  firstName: string;
  lastName: string;
  passwordHash: string;
  phoneNumber?: string;
  preferredLanguage?: string;
  isActive: boolean;
  emailVerified: boolean;
  phoneVerified: boolean;
}

export interface Role extends BaseEntity {
  roleId: number;
  code: string;
  name: string;
  description?: string;
  isSystemRole: boolean;
}

export interface Permission extends BaseEntity {
  permissionId: number;
  code: string;
  name: string;
  description?: string;
  category: string;
}

export interface TreeNode extends BaseEntity {
  treeNodeId: number;
  name: string;
  code: string;
  description?: string;
  treeNodeTypeId: number;
  parentId?: number;
  orderIndex: number;
  path: string;
  isActive: boolean;
}

export interface Media {
  mediaId: number;
  url: string;
  mediaTypeId: number;
  uploadedBy: number;
  uploadedAt: string;
  rowVersion?: string;
}

export interface Question extends BaseEntity {
  questionId: number;
  text: string;
  questionTypeId: number;
  difficultyLevelId: number;
  solution?: string;
  estimatedTimeSec?: number;
  points?: number;
  questionStatusId: number;
  primaryTreeNodeId: number;
  version?: string;
  tags?: string;
  metadata?: string;
  originalQuestionId?: number;
}

export interface Answer extends BaseEntity {
  answerId: number;
  questionId: number;
  text: string;
  isCorrect: boolean;
}

export interface Exam extends BaseEntity {
  examId: number;
  title: string;
  description?: string;
  instructions?: string;
  durationMinutes: number;
  passingScore?: number;
  maxScore: number;
  isRandomized: boolean;
  allowReview: boolean;
  showResults: boolean;
  startDate?: string;
  endDate?: string;
  status: string;
}

// ================= Junction Tables =================
export interface RolePermission {
  roleId: number;
  permissionId: number;
  assignedAt: string;
  assignedBy?: number;
}

export interface UserRole {
  userId: number;
  roleId: number;
  assignedAt: string;
  assignedBy?: number;
}

export interface QuestionMedia {
  questionId: number;
  mediaId: number;
}

export interface AnswerMedia {
  answerId: number;
  mediaId: number;
}

export interface ExamQuestion {
  examId: number;
  questionId: number;
  orderIndex: number;
  points: number;
}

// ================= Authentication Tables =================
export interface RefreshTokens extends BaseEntity {
  refreshTokenId: number;
  tokenHash: string;
  userId: number;
  issuedAt: string;
  expiresAt: string;
  revokedAt?: string;
  replacedByToken?: string;
  reasonRevoked?: string;
}

export interface LoginAttempt {
  loginAttemptId: number;
  username: string;
  userId?: number;
  timestamp: string;
  success: boolean;
  ipAddress?: string;
  userAgent?: string;
  failureReason?: string;
  attemptLocation?: string;
  createdAt: string;
  isDeleted: boolean;
}

// ================= DTO Types for API =================
export interface UserDto {
  userId: number;
  username: string;
  email: string;
  firstName: string;
  lastName: string;
  fullName: string;
  phoneNumber?: string;
  preferredLanguage?: string;
  isActive: boolean;
  emailVerified: boolean;
  phoneVerified: boolean;
  roles: string[];
  permissions: string[];
  createdAt: string;
  updatedAt?: string;
}

export interface CreateUserDto {
  username: string;
  email: string;
  firstName: string;
  lastName: string;
  password: string;
  phoneNumber?: string;
  preferredLanguage?: string;
  roleIds?: number[];
}

export interface UpdateUserDto {
  username?: string;
  email?: string;
  firstName?: string;
  lastName?: string;
  phoneNumber?: string;
  preferredLanguage?: string;
  isActive?: boolean;
  roleIds?: number[];
}

export interface RoleDto {
  roleId: number;
  code: string;
  name: string;
  description?: string;
  isSystemRole: boolean;
  permissions: string[];
  createdAt: string;
  updatedAt?: string;
}

export interface CreateRoleDto {
  code: string;
  name: string;
  description?: string;
  permissionIds?: number[];
}

export interface UpdateRoleDto {
  name?: string;
  description?: string;
  permissionIds?: number[];
}

export interface QuestionDto {
  questionId: number;
  text: string;
  questionType: string;
  difficultyLevel: string;
  solution?: string;
  estimatedTimeSec?: number;
  points?: number;
  questionStatus: string;
  primaryTreeNode?: string;
  version?: string;
  tags?: string[];
  answers: AnswerDto[];
  createdAt: string;
  updatedAt?: string;
}

export interface AnswerDto {
  answerId: number;
  text: string;
  isCorrect: boolean;
}

export interface CreateQuestionDto {
  text: string;
  questionTypeId: number;
  difficultyLevelId: number;
  solution?: string;
  estimatedTimeSec?: number;
  points?: number;
  questionStatusId: number;
  primaryTreeNodeId: number;
  version?: string;
  tags?: string;
  answers: CreateAnswerDto[];
}

export interface CreateAnswerDto {
  text: string;
  isCorrect: boolean;
}

// ================= Pagination and Filtering =================
export interface PagedResult<T> {
  data: T[];
  total: number;
  page: number;
  pageSize: number;
  totalPages: number;
  hasNext: boolean;
  hasPrevious: boolean;
}

export interface UserFilterOptions {
  search?: string;
  isActive?: boolean;
  roleId?: number;
  emailVerified?: boolean;
  createdFrom?: string;
  createdTo?: string;
}

export interface QuestionFilterOptions {
  search?: string;
  questionTypeId?: number;
  difficultyLevelId?: number;
  questionStatusId?: number;
  treeNodeId?: number;
  tags?: string[];
  createdFrom?: string;
  createdTo?: string;
}

// ================= API Response Types =================
export interface ApiResponse<T = any> {
  success: boolean;
  message?: string;
  data?: T;
  errors?: string[];
}

export interface SuccessResponse<T = any> {
  success: true;
  message?: string;
  data: T;
}

export interface ErrorResponse {
  success: false;
  message: string;
  errors?: string[];
  statusCode?: number;
}
