import axios from 'axios';

class AxiosProvider {
    constructor() {
        this.axiosClient = axios.create({
            baseURL: import.meta.env.VITE_API_BASE_URL || 'http://localhost:5000/api/',
            headers: {
                'Content-Type': 'application/json',
            },
        });

        this.axiosClient.interceptors.request.use(
            (config) => {
                const token = localStorage.getItem('authToken');
                if (token) {
                    config.headers.Authorization = `Bearer ${token}`;
                }
                return config;
            },
            (error) => Promise.reject(error)
        );

        this.axiosClient.interceptors.response.use(
            (response) => response,
            async (error) => {
                const originalRequest = error.config;

                if (error.response?.status === 401 && !originalRequest._retry) {
                    originalRequest._retry = true;

                    try {
                        const newToken = await this.refreshToken();
                        if (newToken) {
                            localStorage.setItem('authToken', newToken);
                            originalRequest.headers.Authorization = `Bearer ${newToken}`;
                            return this.axiosClient(originalRequest);
                        }
                    } catch (refreshError) {
                        console.error('Token refresh failed:', refreshError);
                        this.logout();
                        return Promise.reject(refreshError);
                    }
                }

                return Promise.reject(error);
            }
        );
    }

    async login({ username, password }) {
        const response = await this.axiosClient.post('auth/login', {
            username,
            password
        });
        return response.data;
    }

    async register({ username, password }) {
        const response = await this.axiosClient.post('auth/register', {
            username,
            password
        });
        return response.data;
    }

    async refreshToken() {
        const refreshToken = localStorage.getItem('refreshToken');
        if (!refreshToken) {
            throw new Error('No refresh token found');
        }

        const response = await this.axiosClient.get(`auth/refresh-token`, {
            params: { token: refreshToken }
        });
        
        return response.data;
    }

    logout() {
        localStorage.removeItem('authToken');
        localStorage.removeItem('refreshToken');
        localStorage.removeItem('userId');
        localStorage.removeItem('username');
        delete this.axiosClient.defaults.headers.common['Authorization'];
        window.location.href = '/';
    }

    setAuthToken(token) {
        if (token) {
            this.axiosClient.defaults.headers.common['Authorization'] = `Bearer ${token}`;
        } else {
            delete this.axiosClient.defaults.headers.common['Authorization'];
        }
    }

    verifyTokenExists() {
        return !!this.axiosClient.defaults.headers.common['Authorization'];
    }

    async listUsers() {
        const response = await this.axiosClient.get('user/users');
        return response.data;
    }

    async getMessageHistory(roomId) {
        const response = await this.axiosClient.get(`chat/messages/${roomId}`);
        return response.data;
    }

    async getRoomsByUserId(userId) {
        const response = await this.axiosClient.get(`chat/rooms/${userId}`);
        return response.data;
    }
}

const axiosInstance = new AxiosProvider();
export default axiosInstance;