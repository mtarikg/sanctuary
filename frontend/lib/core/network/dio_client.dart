import 'package:dio/dio.dart';
import 'package:flutter/foundation.dart';
import 'package:flutter_secure_storage/flutter_secure_storage.dart';
import 'package:frontend/core/constants/constants.dart';
import 'package:frontend/core/utils/api_exception.dart';

class DioClient {
  DioClient._();
  static final instance = DioClient._();
  late final Dio dio = _createDio();

  Dio _createDio() {
    final d = Dio(
      BaseOptions(
        baseUrl: AppConstants.baseUrl,
        contentType: 'application/json',
        connectTimeout: const Duration(seconds: 10),
        receiveTimeout: const Duration(seconds: 10),
      ),
    );

    d.interceptors.add(_AuthInterceptor());
    if (kDebugMode) {
      d.interceptors.add(LogInterceptor(requestBody: true, responseBody: true));
    }

    return d;
  }
}

class _AuthInterceptor extends Interceptor {
  static const _storage = FlutterSecureStorage();

  @override
  Future<void> onRequest(
    RequestOptions options,
    RequestInterceptorHandler handler,
  ) async {
    final token = await _storage.read(key: AppConstants.authTokenKey);
    if (token != null) {
      options.headers['Authorization'] = 'Bearer $token';
    }
    handler.next(options);
  }

  @override
  void onError(DioException err, ErrorInterceptorHandler handler) {
    final statusCode = err.response?.statusCode ?? 0;
    final body = err.response?.data;

    String message = 'Could not reach the server';
    if (body is Map<String, dynamic>) {
      message =
          body['detail'] as String? ?? body['title'] as String? ?? message;
    }

    handler.reject(
      DioException(
        requestOptions: err.requestOptions,
        error: ApiException(statusCode, message),
        response: err.response,
        type: err.type,
      ),
    );
  }
}
