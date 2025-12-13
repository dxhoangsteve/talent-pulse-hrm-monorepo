import React, { useState } from 'react';
import {
  View,
  Text,
  TextInput,
  TouchableOpacity,
  StyleSheet,
  Dimensions,
  Platform,
  KeyboardAvoidingView,
  Alert,
  ActivityIndicator,
  Image,
  ScrollView,
} from 'react-native';
import { StatusBar } from 'expo-status-bar';
import { LinearGradient } from 'expo-linear-gradient';
import { Mail, Lock, Eye, EyeOff, Sparkles } from 'lucide-react-native';
import { Colors, Spacing, FontSize, BorderRadius } from '../constants/theme';
import { API_CONFIG, ENDPOINTS } from '../config/api.config';
import { useAuth } from '../context/AuthContext';

const { width, height } = Dimensions.get('window');

export default function LoginScreen() {
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [showPassword, setShowPassword] = useState(false);
  const [isLoading, setIsLoading] = useState(false);
  const [errors, setErrors] = useState<{ email?: string; password?: string }>({});
  const [apiError, setApiError] = useState<string | null>(null);
  
  const { login } = useAuth();

  const validateForm = () => {
    const newErrors: { email?: string; password?: string } = {};
    
    if (!email) {
      newErrors.email = 'Email là bắt buộc';
    }
    
    if (!password) {
      newErrors.password = 'Mật khẩu là bắt buộc';
    } else if (password.length < 6) {
      newErrors.password = 'Mật khẩu phải có ít nhất 6 ký tự';
    }
    
    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  const handleLogin = async () => {
    if (!validateForm()) return;
    
    setIsLoading(true);
    setApiError(null);
    
    console.log(`Attempting login to: ${API_CONFIG.API_URL}${ENDPOINTS.LOGIN}`);
    console.log('Credentials:', { userName: email, password: '***' });

    try {
      const response = await fetch(`${API_CONFIG.API_URL}${ENDPOINTS.LOGIN}`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify({
          userName: email,
          password: password,
        }),
      });
      
      console.log('Response status:', response.status);
      const text = await response.text();
      console.log('Response body:', text);

      let result;
      try {
        result = JSON.parse(text);
      } catch (e) {
        throw new Error('Invalid JSON response: ' + text);
      }
      
      if (result.isSuccessed) {
        const { token, role } = result.resultObj;
        console.log('Login success!', { token: token.substring(0, 10) + '...', role });
        await login(token);
      } else {
        setApiError(result.message || 'Đăng nhập thất bại');
      }
    } catch (error: any) {
      console.error('Login error full:', error);
      setApiError(`Lỗi kết nối: ${error.message}`);
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <View style={styles.container}>
      <StatusBar style="light" />
      
      {/* Header Gradient */}
      <LinearGradient
        colors={[Colors.gradientStart, Colors.gradientEnd, '#A855F7']}
        start={{ x: 0, y: 0 }}
        end={{ x: 1, y: 1 }}
        style={styles.headerGradient}
      />
      
      {/* Decorative Circles */}
      <View style={[styles.circle, styles.circle1]} />
      <View style={[styles.circle, styles.circle2]} />

      <ScrollView 
        contentContainerStyle={styles.scrollContent} 
        showsVerticalScrollIndicator={false}
        keyboardShouldPersistTaps="handled"
      >
        <View style={styles.contentContainer}>
          {/* Logo Section */}
          <View style={styles.logoSection}>
            <View style={styles.logoContainer}>
              <Sparkles size={40} color="white" />
            </View>
            <Text style={styles.appName}>TalentPulse</Text>
            <Text style={styles.tagline}>Hệ thống quản lý nhân sự</Text>
          </View>

          {/* Login Card */}
          <View style={styles.card}>
            <View style={styles.cardHeader}>
              <Text style={styles.cardTitle}>Đăng nhập</Text>
              <Text style={styles.cardSubtitle}>Chào mừng bạn trở lại!</Text>
            </View>

            {/* Email Field */}
            <View style={styles.inputContainer}>
              <Text style={styles.label}>Email/Tài khoản</Text>
              <View style={[styles.inputWrapper, errors.email && styles.inputError]}>
                <Mail size={20} color={Colors.textSecondary} style={styles.inputIcon} />
                <TextInput
                  style={styles.input}
                  placeholder="Nhập tài khoản"
                  placeholderTextColor={Colors.textLight}
                  value={email}
                  onChangeText={(t) => {
                    setEmail(t);
                    if (errors.email) setErrors({ ...errors, email: undefined });
                  }}
                  autoCapitalize="none"
                />
              </View>
              {errors.email && <Text style={styles.errorText}>{errors.email}</Text>}
            </View>

            {/* Password Field */}
            <View style={styles.inputContainer}>
              <Text style={styles.label}>Mật khẩu</Text>
              <View style={[styles.inputWrapper, errors.password && styles.inputError]}>
                <Lock size={20} color={Colors.textSecondary} style={styles.inputIcon} />
                <TextInput
                  style={styles.input}
                  placeholder="Nhập mật khẩu"
                  placeholderTextColor={Colors.textLight}
                  value={password}
                  onChangeText={(t) => {
                    setPassword(t);
                    if (errors.password) setErrors({ ...errors, password: undefined });
                  }}
                  secureTextEntry={!showPassword}
                />
                <TouchableOpacity onPress={() => setShowPassword(!showPassword)}>
                  {showPassword ? (
                    <EyeOff size={20} color={Colors.textSecondary} />
                  ) : (
                    <Eye size={20} color={Colors.textSecondary} />
                  )}
                </TouchableOpacity>
              </View>
              {errors.password && <Text style={styles.errorText}>{errors.password}</Text>}
            </View>

            {/* Login Button */}
            <TouchableOpacity
              style={[styles.loginButton, isLoading && styles.loginButtonDisabled]}
              onPress={handleLogin}
              disabled={isLoading}
            >
              {isLoading ? (
                <ActivityIndicator color="white" />
              ) : (
                <Text style={styles.loginButtonText}>Đăng nhập</Text>
              )}
            </TouchableOpacity>

            {/* API Error */}
            {apiError && (
              <View style={styles.errorContainer}>
                <Text style={styles.apiErrorText}>{apiError}</Text>
              </View>
            )}
          </View>
        </View>
      </ScrollView>
    </View>
  );
}

const styles = StyleSheet.create({
  container: {
    flex: 1,
    backgroundColor: Colors.background,
  },
  headerGradient: {
    position: 'absolute',
    top: 0,
    left: 0,
    right: 0,
    height: height * 0.42,
    borderBottomLeftRadius: 32,
    borderBottomRightRadius: 32,
  },
  circle: {
    position: 'absolute',
    borderRadius: 999,
  },
  circle1: {
    width: 180,
    height: 180,
    backgroundColor: 'rgba(255,255,255,0.1)',
    top: -40,
    right: -40,
  },
  circle2: {
    width: 120,
    height: 120,
    backgroundColor: 'rgba(255,255,255,0.08)',
    top: 80,
    left: -40,
  },
  scrollContent: {
    flexGrow: 1,
  },
  contentContainer: {
    flex: 1,
    paddingTop: height * 0.08,
    paddingBottom: Spacing.xl,
  },
  logoSection: {
    alignItems: 'center',
    marginBottom: Spacing.xl,
  },
  logoContainer: {
    width: 90,
    height: 90,
    borderRadius: BorderRadius.lg,
    backgroundColor: 'rgba(255,255,255,0.2)',
    borderWidth: 2,
    borderColor: 'rgba(255,255,255,0.3)',
    alignItems: 'center',
    justifyContent: 'center',
    marginBottom: Spacing.md,
  },
  appName: {
    color: 'white',
    fontSize: 32,
    fontWeight: 'bold',
  },
  tagline: {
    color: 'rgba(255,255,255,0.85)',
    fontSize: FontSize.md,
  },
  card: {
    flex: 1,
    marginHorizontal: Spacing.lg,
    padding: Spacing.lg,
    borderRadius: BorderRadius.lg,
    backgroundColor: 'white',
    shadowColor: '#000',
    shadowOffset: { width: 0, height: 10 },
    shadowOpacity: 0.1,
    shadowRadius: 20,
    elevation: 20,
  },
  cardHeader: {
    marginBottom: Spacing.lg,
  },
  cardTitle: {
    fontSize: FontSize.xl,
    fontWeight: 'bold',
    color: Colors.text,
    marginBottom: Spacing.xs,
  },
  cardSubtitle: {
    color: Colors.textSecondary,
    fontSize: FontSize.md,
  },
  inputContainer: {
    marginBottom: Spacing.md,
  },
  label: {
    fontSize: FontSize.sm,
    fontWeight: '600',
    color: Colors.textSecondary,
    marginBottom: Spacing.xs,
  },
  inputWrapper: {
    flexDirection: 'row',
    alignItems: 'center',
    backgroundColor: Colors.background,
    borderRadius: BorderRadius.sm,
    borderWidth: 1,
    borderColor: Colors.border,
    paddingHorizontal: Spacing.md,
    height: 50,
  },
  inputError: {
    borderColor: Colors.error,
  },
  inputIcon: {
    marginRight: Spacing.sm,
  },
  input: {
    flex: 1,
    fontSize: FontSize.md,
    color: Colors.text,
  },
  errorText: {
    color: Colors.error,
    fontSize: FontSize.xs,
    marginTop: Spacing.xs,
  },
  forgotPassword: {
    alignItems: 'flex-end',
    marginBottom: Spacing.lg,
  },
  forgotPasswordText: {
    color: Colors.primary,
    fontWeight: '600',
    fontSize: FontSize.sm,
  },
  loginButton: {
    backgroundColor: Colors.primary,
    height: 50,
    borderRadius: BorderRadius.sm,
    justifyContent: 'center',
    alignItems: 'center',
  },
  loginButtonDisabled: {
    backgroundColor: Colors.primaryLight,
  },
  loginButtonText: {
    color: 'white',
    fontWeight: 'bold',
    fontSize: FontSize.md,
  },
  errorContainer: {
    backgroundColor: '#FEE2E2',
    padding: Spacing.sm,
    borderRadius: BorderRadius.sm,
    marginTop: Spacing.sm,
    alignItems: 'center',
  },
  apiErrorText: {
    color: Colors.error,
    fontSize: FontSize.sm,
    textAlign: 'center',
  },
  dividerContainer: {
    flexDirection: 'row',
    alignItems: 'center',
    marginVertical: Spacing.lg,
  },
  dividerLine: {
    flex: 1,
    height: 1,
    backgroundColor: Colors.border,
  },
  dividerText: {
    paddingHorizontal: Spacing.md,
    color: Colors.textSecondary,
    fontSize: FontSize.sm,
  },
  socialContainer: {
    flexDirection: 'row',
    gap: Spacing.md,
  },
  socialButton: {
    flex: 1,
    flexDirection: 'row',
    alignItems: 'center',
    justifyContent: 'center',
    padding: Spacing.md,
    backgroundColor: Colors.background,
    borderWidth: 1,
    borderColor: Colors.border,
    borderRadius: BorderRadius.sm,
    gap: Spacing.sm,
  },
  socialText: {
    fontWeight: '600',
    color: Colors.textSecondary,
  },
  footer: {
    alignItems: 'center',
    paddingVertical: Spacing.md,
  },
  footerText: {
    color: Colors.textSecondary,
    fontSize: FontSize.xs,
  },
});
