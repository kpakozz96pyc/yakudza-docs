export interface Ingredient {
  id?: number;
  name: string;
  weightGrams: number;
}

export interface DishListItem {
  id: number;
  name: string;
  description: string;
  hasImage: boolean;
}

export interface DishDetails {
  id: number;
  name: string;
  description: string;
  hasImage: boolean;
  ingredients: Ingredient[];
}

export interface PagedResult<T> {
  items: T[];
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages: number;
}

export interface CreateDishRequest {
  name: string;
  description: string;
  imageBase64?: string;
  ingredients: Ingredient[];
}

export interface UpdateDishRequest {
  name: string;
  description: string;
  imageBase64?: string;
  ingredients: Ingredient[];
}
