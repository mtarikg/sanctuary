import 'package:frontend/core/utils/api_exception.dart';

String mapError(Object e) {
  if (e is ApiException) return e.message;
  return 'Could not reach the server.';
}
