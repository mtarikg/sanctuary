import 'dart:convert';
import 'package:flutter_secure_storage/flutter_secure_storage.dart';
import 'package:frontend/core/utils/error_mapper.dart';
import 'package:frontend/features/auth/models/auth_result.dart';
import 'package:http/http.dart' as http;
import '../../../core/constants/constants.dart';

class AuthService {
  static const _storage = FlutterSecureStorage();

  Future<AuthResult> login(String email, String password) async {
    try {
      final requestBody = {'email': email, 'password': password};

      final response = await http.post(
        Uri.parse('${AppConstants.baseUrl}/auth/login'),
        headers: {'Content-Type': 'application/json'},
        body: jsonEncode(requestBody),
      );

      if (response.statusCode == 200) {
        final token = jsonDecode(response.body)['token'] as String;
        return AuthResult.success(token);
      } else if (response.statusCode == 401) {
        return const AuthResult.failure('Incorrect email or password.');
      } else {
        final body = response.body;
        final message = body.isNotEmpty
            ? jsonDecode(body)['errors']?.toString() ?? 'Something went wrong.'
            : 'Something went wrong.';
        return AuthResult.failure(message);
      }
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
      final response = await http.post(
        Uri.parse('${AppConstants.baseUrl}/auth/register'),
        headers: {'Content-Type': 'application/json'},
        body: jsonEncode({
          'username': username,
          'email': email,
          'password': password,
        }),
      );

      if (response.statusCode == 200 || response.statusCode == 201) {
        final token = jsonDecode(response.body)['token'] as String;
        return AuthResult.success(token);
      } else {
        final body = response.body;
        final message = body.isNotEmpty
            ? jsonDecode(body)['errors']?.toString() ?? 'Something went wrong.'
            : 'Something went wrong.';
        return AuthResult.failure(message);
      }
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
