// API Configuration
// Thay đổi IP này thành IP của máy chạy Backend
const LOCAL_IP = '192.168.1.13';
const API_PORT = '5294';

export const API_BASE_URL = `http://${LOCAL_IP}:${API_PORT}/api`;

// For production: 'https://your-api-domain.com/api'
// For Android emulator: use 'http://10.0.2.2:PORT/api'
// For iOS simulator: use 'http://localhost:PORT/api'
