export interface RegisterRequest {
	email: string
	password: string
	confirmPassword: string
	name: string
	lastName: string
	phoneNumber: string
	country: string
}

export interface AuthResponse {
	message: string
	token: string
	email: string
	role: string
}

export interface RegisterFormState extends RegisterRequest {}