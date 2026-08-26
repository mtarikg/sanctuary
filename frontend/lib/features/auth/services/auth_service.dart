import 'package:dio/dio.dart';
import '../../../core/network/dio_client.dart';
import '../models/auth_response.dart';
import '../models/login_request.dart';
import '../models/register_request.dart';
import '../../../core/constants/constants.dart';
import '../../../core/network/secure_storage.dart';

class AuthService {
  final _storage = SecureStorage.instance;
  final Dio _dio = DioClient.instance.dio;

  Future<AuthResponse> login(String email, String password) async {
    final response = await _dio.post(
      '/auth/login',
      data: LoginRequest(email: email, password: password).toJson(),
    );

    return AuthResponse.fromJson(response.data as Map<String, dynamic>);
  }

  Future<AuthResponse> register(
    String username,
    String email,
    String password,
  ) async {
    final response = await _dio.post(
      '/auth/register',
      data: RegisterRequest(
        username: username,
        email: email,
        password: password,
      ).toJson(),
    );

    return AuthResponse.fromJson(response.data as Map<String, dynamic>);
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
