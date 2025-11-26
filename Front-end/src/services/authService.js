import api from "./api";

const loginCliente = async (email, senha) => {
  const { data } = await api.post("/ClienteAuth/login", { email, senha });
  return data; // { token, expires }
};

const loginAdmin = async (username, password) => {
  const { data } = await api.post("/AdminAuth/login", { username, password });
  return data; // { token, expires }
};

export default {
  loginCliente,
  loginAdmin,
};