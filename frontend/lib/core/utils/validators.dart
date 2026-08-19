String? validateAuthForm({
  required bool isLogin,
  required String username,
  required String email,
  required String password,
  required String confirmPassword,
}) {
  if (!isLogin && username.trim().isEmpty) {
    return 'Enter a username.';
  }

  if (email.trim().isEmpty) {
    return 'Enter an email address.';
  }

  if (password.length < 8) {
    return 'Password must be at least 8 characters long.';
  }

  if (!isLogin && password != confirmPassword) {
    return 'Passwords do not match.';
  }

  return null;
}
