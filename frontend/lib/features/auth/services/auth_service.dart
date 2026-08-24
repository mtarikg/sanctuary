import 'package:dio/dio.dart';
import 'package:flutter_secure_storage/flutter_secure_storage.dart';
import 'package:frontend/core/network/dio_client.dart';
import 'package:frontend/core/utils/error_mapper.dart';
import 'package:frontend/features/auth/models/auth_result.dart';
import 'package:frontend/features/auth/models/login_request.dart';
import 'package:frontend/features/auth/models/register_request.dart';
import '../../../core/constants/constants.dart';

class AuthService {
  static const _storage = FlutterSecureStorage();
  final Dio _dio = DioClient.instance.dio;

  Future<AuthResult> login(String email, String password) async {
    try {
      final response = await _dio.post(
        '/auth/login',
        data: LoginRequest(email: email, password: password).toJson(),
      );

      final token = response.data['token'] as String;
      return AuthResult.success(token);
    } on DioException catch (e) {
      if (e.response?.statusCode == 401) {
        return const AuthResult.failure('Incorrect email or password.');
      }
      return AuthResult.failure(mapError(e.error ?? e));
    } catch (e) {
      return AuthResult.failure(mapError(e));
    }
  }

  Future<AuthResult> register(
    String username,
    String email,
    String password,
  ) async {
    try {
      final response = await _dio.post(
        '/auth/register',
        data: RegisterRequest(
          username: username,
          email: email,
          password: password,
        ).toJson(),
      );

      final token = response.data['token'] as String;
      return AuthResult.success(token);
    } on DioException catch (e) {
      return AuthResult.failure(mapError(e.error ?? e));
    } catch (e) {
      return AuthResult.failure(mapError(e));
    }
  }

  Future<void> saveToken(String token) async {
    await _storage.write(key: AppConstants.authTokenKey, value: token);
  }

  Future<void> clearToken() async {
    await _storage.delete(key: AppConstants.authTokenKey);
  }

  Future<String?> getToken() async {
    return _storage.read(key: AppConstants.authTokenKey);
  }
}
