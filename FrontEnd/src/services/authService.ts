import AsyncStorage from '@react-native-async-storage/async-storage';
import { jwtDecode } from 'jwt-decode';
import { DecodedToken, UserRole } from '../types/types';

const TOKEN_KEY = 'auth_token';

export const authService = {
  saveToken: async (token: string) => {
    try {
      await AsyncStorage.setItem(TOKEN_KEY, token);
    } catch (error) {
      console.error('Error saving token:', error);
    }
  },

  getToken: async () => {
    try {
      return await AsyncStorage.getItem(TOKEN_KEY);
    } catch (error) {
      console.error('Error getting token:', error);
      return null;
    }
  },

  removeToken: async () => {
    try {
      await AsyncStorage.removeItem(TOKEN_KEY);
    } catch (error) {
      console.error('Error removing token:', error);
    }
  },

  decodeToken: (token: string): DecodedToken | null => {
    try {
      return jwtDecode<DecodedToken>(token);
    } catch (error) {
      console.error('Error decoding token:', error);
      return null;
    }
  },

  isTokenExpired: (token: string): boolean => {
    try {
      const decoded = jwtDecode<DecodedToken>(token);
      if (decoded.exp < Date.now() / 1000) {
        return true;
      }
      return false;
    } catch (e) {
      return true;
    }
  },
  
  extractRole: (decoded: DecodedToken): UserRole => {
    // Check for standard role claim or Microsoft identity role claim
    const roleClaim = decoded.role || 
                     (decoded as any)['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'];

    if (!roleClaim) return 'Employee';
    
    const roles = Array.isArray(roleClaim) ? roleClaim : roleClaim.split(';');
    
    if (roles.includes('SuperAdmin') || roles.includes('Admin')) return 'Admin';
    if (roles.includes('HR')) return 'HR'; // Can be mapped to Admin or Employee Dashboard
    if (roles.includes('Manager')) return 'Manager';
    
    return 'Employee';
  },
  
  isAdmin: (decoded: DecodedToken): boolean => {
      const role = authService.extractRole(decoded);
      return role === 'Admin' || role === 'SuperAdmin';
  }
};
