import type {
  DishListItem,
  DishDetails,
  PagedResult,
  CreateDishRequest,
  UpdateDishRequest,
} from '../types/api';
import type { LoginRequest, LoginResponse, InitAdminRequest } from '../types/auth';

// Use relative URL in development (proxied by Vite) or configured URL in production
const API_BASE_URL = import.meta.env.DEV
  ? '/api'
  : (import.meta.env.VITE_API_BASE_URL || '/api');

// Get auth token from localStorage
function getAuthToken(): string | null {
  const authUser = localStorage.getItem('yakudza_auth_user');
  if (!authUser) return null;
  try {
    const parsed = JSON.parse(authUser);
    return parsed.token;
  } catch {
    return null;
  }
}

// Add Authorization header if token exists
function getHeaders(): HeadersInit {
  const headers: HeadersInit = {
    'Content-Type': 'application/json',
  };

  const token = getAuthToken();
  if (token) {
    headers['Authorization'] = `Bearer ${token}`;
  }

  return headers;
}

class ApiError extends Error {
  status: number;

  constructor(status: number, message: string) {
    super(message);
    this.status = status;
    this.name = 'ApiError';
  }
}

async function handleResponse<T>(response: Response): Promise<T> {
  if (!response.ok) {
    const errorText = await response.text();
    throw new ApiError(response.status, errorText || response.statusText);
  }
  return response.json();
}

export const dishesApi = {
  async getAll(
    page: number = 1,
    pageSize: number = 10,
    search?: string
  ): Promise<PagedResult<DishListItem>> {
    const params = new URLSearchParams({
      page: page.toString(),
      pageSize: pageSize.toString(),
    });

    if (search) {
      params.append('search', search);
    }

    const response = await fetch(`${API_BASE_URL}/dishes?${params}`);
    return handleResponse<PagedResult<DishListItem>>(response);
  },

  async getById(id: number): Promise<DishDetails> {
    const response = await fetch(`${API_BASE_URL}/dishes/${id}`);
    return handleResponse<DishDetails>(response);
  },

  async create(dish: CreateDishRequest): Promise<DishDetails> {
    const response = await fetch(`${API_BASE_URL}/dishes`, {
      method: 'POST',
      headers: getHeaders(),
      body: JSON.stringify(dish),
    });
    return handleResponse<DishDetails>(response);
  },

  async update(id: number, dish: UpdateDishRequest): Promise<DishDetails> {
    const response = await fetch(`${API_BASE_URL}/dishes/${id}`, {
      method: 'PUT',
      headers: getHeaders(),
      body: JSON.stringify(dish),
    });
    return handleResponse<DishDetails>(response);
  },

  async delete(id: number): Promise<void> {
    const response = await fetch(`${API_BASE_URL}/dishes/${id}`, {
      method: 'DELETE',
      headers: getHeaders(),
    });

    if (!response.ok) {
      const errorText = await response.text();
      throw new ApiError(response.status, errorText || response.statusText);
    }
  },

  getImageUrl(id: number): string {
    return `${API_BASE_URL}/dishes/${id}/image`;
  },
};

export const authApi = {
  async login(credentials: LoginRequest): Promise<LoginResponse> {
    const response = await fetch(`${API_BASE_URL}/auth/login`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify(credentials),
    });
    return handleResponse<LoginResponse>(response);
  },

  async initAdmin(data: InitAdminRequest): Promise<LoginResponse> {
    const response = await fetch(`${API_BASE_URL}/auth/init-admin`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify(data),
    });
    return handleResponse<LoginResponse>(response);
  },

  async checkAdminExists(): Promise<boolean> {
    const response = await fetch(`${API_BASE_URL}/auth/check-admin`);
    return handleResponse<boolean>(response);
  },
};
