export interface LoginRequest {
  login: string;
  password: string;
}

export interface LoginResponse {
  token: string;
  login: string;
  role: string;
}

export interface InitAdminRequest {
  login: string;
  password: string;
}

export interface AuthUser {
  login: string;
  role: string;
  token: string;
}
