import 'package:freezed_annotation/freezed_annotation.dart';

part 'auth_result.freezed.dart';

@freezed
sealed class AuthResult with _$AuthResult {
  const factory AuthResult.success(String token) = AuthSuccess;
  const factory AuthResult.failure(String error) = AuthFailure;
}
