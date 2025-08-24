/**
 * User Management Hook
 * Custom React hook for managing user operations with state management,
 * error handling, loading states, and data caching
 */

import { useState, useEffect, useCallback } from 'react';
import { User, CreateUserRequest, UpdateUserRequest } from '../models/user.types';
import { userService } from '../services/userService';

export const useUserManagement = () => {
  const [users, setUsers] = useState<User[]>([]);
  const [total, setTotal] = useState(0);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const load = useCallback(async (page = 1, filter = '') => {
    setLoading(true);
    setError(null);
    try {
      const { items, total } = await userService.list(page, filter);
      setUsers(items);
      setTotal(total);
    } catch (e) {
      setError((e as Error).message);
    } finally {
      setLoading(false);
    }
  }, []);

  const create = useCallback(async (data: CreateUserRequest) => {
    try {
      await userService.create(data);
      await load(); // Refresh the list
    } catch (e) {
      setError((e as Error).message);
      throw e;
    }
  }, [load]);

  const update = useCallback(async (data: UpdateUserRequest) => {
    try {
      await userService.update(data);
      await load(); // Refresh the list
    } catch (e) {
      setError((e as Error).message);
      throw e;
    }
  }, [load]);

  const remove = useCallback(async (id: string) => {
    try {
      await userService.remove(id);
      await load(); // Refresh the list
    } catch (e) {
      setError((e as Error).message);
      throw e;
    }
  }, [load]);

  useEffect(() => {
    load();
  }, [load]);

  return { users, total, loading, error, load, create, update, remove };
};
