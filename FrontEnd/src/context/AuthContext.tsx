import React, { createContext, useState, useEffect, useContext } from 'react';
import { authService } from '../services/authService';
import { DecodedToken } from '../types/types';

interface AuthContextData {
  token: string | null;
  user: DecodedToken | null;
  isLoading: boolean;
  isAdmin: boolean;
  login: (token: string) => Promise<void>;
  logout: () => Promise<void>;
}

const AuthContext = createContext<AuthContextData>({} as AuthContextData);

export const AuthProvider: React.FC<{ children: React.ReactNode }> = ({ children }) => {
  const [token, setToken] = useState<string | null>(null);
  const [user, setUser] = useState<DecodedToken | null>(null);
  const [isLoading, setIsLoading] = useState(true);

  useEffect(() => {
    loadToken();
  }, []);

  const loadToken = async () => {
    try {
      const storedToken = await authService.getToken();
      if (storedToken && !authService.isTokenExpired(storedToken)) {
        const decoded = authService.decodeToken(storedToken);
        setToken(storedToken);
        setUser(decoded);
      } else {
        await authService.removeToken();
        setToken(null);
        setUser(null);
      }
    } catch (e) {
      console.error(e);
    } finally {
      setIsLoading(false);
    }
  };

  const login = async (newToken: string) => {
    await authService.saveToken(newToken);
    const decoded = authService.decodeToken(newToken);
    setToken(newToken);
    setUser(decoded);
  };

  const logout = async () => {
    await authService.removeToken();
    setToken(null);
    setUser(null);
  };
  
  const isAdmin = user ? authService.isAdmin(user) : false;

  return (
    <AuthContext.Provider value={{ token, user, isLoading, isAdmin, login, logout }}>
      {children}
    </AuthContext.Provider>
  );
};

export const useAuth = () => useContext(AuthContext);
