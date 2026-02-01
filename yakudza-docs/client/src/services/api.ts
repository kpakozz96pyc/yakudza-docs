import type {
  DishListItem,
  DishDetails,
  PagedResult,
  CreateDishRequest,
  UpdateDishRequest,
} from '../types/api';

// Use relative URL in development (proxied by Vite) or absolute URL in production
const API_BASE_URL = import.meta.env.DEV ? '/api' : 'http://localhost:8080/api';

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
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify(dish),
    });
    return handleResponse<DishDetails>(response);
  },

  async update(id: number, dish: UpdateDishRequest): Promise<DishDetails> {
    const response = await fetch(`${API_BASE_URL}/dishes/${id}`, {
      method: 'PUT',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify(dish),
    });
    return handleResponse<DishDetails>(response);
  },

  async delete(id: number): Promise<void> {
    const response = await fetch(`${API_BASE_URL}/dishes/${id}`, {
      method: 'DELETE',
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
