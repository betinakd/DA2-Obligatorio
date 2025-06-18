export interface ErrorResponse {
  error: ErrorResponse;
  status: number;
  innerCode: number;
  message: string;
}