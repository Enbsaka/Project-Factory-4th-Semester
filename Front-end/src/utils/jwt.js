// Decodifica JWT de forma simples (sem validação criptográfica)
// Suporta base64 URL-safe
export function parseJwt(token) {
  try {
    const parts = token?.split?.('.') || [];
    if (parts.length < 2) return null;
    const base64 = parts[1]
      .replace(/-/g, '+')
      .replace(/_/g, '/');
    const jsonPayload = decodeURIComponent(
      atob(base64)
        .split('')
        .map(function (c) {
          return '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2);
        })
        .join('')
    );
    return JSON.parse(jsonPayload);
  } catch (e) {
    console.error('Falha ao decodificar JWT:', e);
    return null;
  }
}