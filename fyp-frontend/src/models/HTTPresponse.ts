export default interface HTTPresponse<T, TError> {
  success: boolean;
  data: T;
  error: TError;
  hasData: boolean;
  hasError: boolean;
  message: string;
  httpCode: string;
}