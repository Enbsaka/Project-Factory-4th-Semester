import axios from "axios";

const api = axios.create({
  baseURL: import.meta.env.VITE_API_URL || "http://localhost:5212/api",
});

// Interceptador de token e correção de FormData
api.interceptors.request.use((config) => {
  const token = localStorage.getItem("token");
  if (token) {
    config.headers = config.headers || {};
    config.headers.Authorization = `Bearer ${token}`;
  }
  // Para FormData, deixar o browser definir o boundary
  if (config.data instanceof FormData) {
    if (config.headers && config.headers["Content-Type"]) {
      delete config.headers["Content-Type"];
    }
  }
  return config;
});

export default api;
