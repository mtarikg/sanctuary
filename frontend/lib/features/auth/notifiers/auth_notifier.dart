import 'package:frontend/features/auth/services/auth_service.dart';
import 'package:frontend/providers/auth_provider.dart';
import 'package:riverpod_annotation/riverpod_annotation.dart';

part 'auth_notifier.g.dart';

@riverpod
class AuthNotifier extends _$AuthNotifier {
  late final AuthService _authService;

  @override
  FutureOr<void> build() {
    _authService = AuthService();
  }

  Future<void> login(String email, String password) async {
    state = await AsyncValue.guard(() async {
      final result = await _authService.login(email, password);
      await _authService.saveToken(result.token);
      ref.read(authTokenProvider.notifier).state = result.token;
    });
  }

  Future<void> register(String username, String email, String password) async {
    state = await AsyncValue.guard(() async {
      final result = await _authService.register(username, email, password);
      await _authService.saveToken(result.token);
      ref.read(authTokenProvider.notifier).state = result.token;
    });
  }
}
