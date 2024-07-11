import { LoginError } from "../interfaces/login-error.interface";
import { LoginSuccess } from "../interfaces/login-success.interface";

export type LoginResponse = LoginSuccess | LoginError;