import 'package:flutter/material.dart';
import 'package:frontend/main.dart';
import 'package:http/http.dart' as http;
import 'dart:convert';

String? authToken;

class AuthScreen extends StatefulWidget {
  const AuthScreen({super.key});

  @override
  State<AuthScreen> createState() => _AuthScreenState();
}

class _AuthScreenState extends State<AuthScreen> {
  bool _isLogin = true;

  final _emailController = TextEditingController();
  final _usernameController = TextEditingController();
  final _passwordController = TextEditingController();
  final _confirmPasswordController = TextEditingController();

  String? _errorMessage;

  @override
  void dispose() {
    _emailController.dispose();
    _usernameController.dispose();
    _passwordController.dispose();
    _confirmPasswordController.dispose();
    super.dispose();
  }

  void _switchMode(bool isLogin) {
    setState(() {
      _isLogin = isLogin;
      _errorMessage = null;

      _usernameController.clear();
      _emailController.clear();
      _passwordController.clear();
      _confirmPasswordController.clear();
    });
  }

  Future<void> _submit() async {
    setState(() => _errorMessage = null);

    if (!_isLogin && _usernameController.text.trim().isEmpty) {
      setState(() => _errorMessage = 'Enter a username.');
      return;
    }

    if (_emailController.text.trim().isEmpty) {
      setState(() => _errorMessage = 'Enter an email address.');
      return;
    }

    if (_passwordController.text.length < 8) {
      setState(
        () => _errorMessage = 'Password must be at least 8 characters long.',
      );
      return;
    }

    if (!_isLogin &&
        _passwordController.text != _confirmPasswordController.text) {
      setState(() => _errorMessage = 'Passwords do not match.');
      return;
    }

    try {
      final url = Uri.parse(
        _isLogin
            ? 'http://10.0.2.2:5002/auth/login'
            : 'http://10.0.2.2:5002/auth/register',
      );

      final body = _isLogin
          ? {
              'email': _emailController.text.trim(),
              'password': _passwordController.text,
            }
          : {
              'username': _usernameController.text.trim(),
              'email': _emailController.text.trim(),
              'password': _passwordController.text,
            };

      final response = await http.post(
        url,
        headers: {'Content-Type': 'application/json'},
        body: jsonEncode(body),
      );

      if (response.statusCode == 200 || response.statusCode == 201) {
        final data = jsonDecode(response.body);
        final token = data['token'];

        authToken = token;

        if (mounted) {
          Navigator.of(context).pushReplacement(
            MaterialPageRoute(builder: (_) => const MainShell()),
          );
        }
      } else if (response.statusCode == 401) {
        setState(() => _errorMessage = 'Incorrect email or password.');
      }   
      else {
        final data = jsonDecode(response.body);
        setState(
          () => _errorMessage =
              data['errors']?.toString() ?? 'Something went wrong.',
        );
      }
    } catch (e) {
      setState(() => _errorMessage = 'Could not reach the server.');
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      body: SafeArea(
        child: SingleChildScrollView(
          padding: const EdgeInsets.symmetric(horizontal: 24, vertical: 32),
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              const SizedBox(height: 16),
              Center(
                child: Column(
                  children: [
                    Text(
                      'Sanctuary',
                      style: Theme.of(context).textTheme.headlineMedium
                          ?.copyWith(fontWeight: FontWeight.w500),
                    ),
                    const SizedBox(height: 4),
                    Text(
                      'Your sanctuary for all your thoughts.',
                      style: TextStyle(fontSize: 12, color: Colors.grey[600]),
                    ),
                  ],
                ),
              ),
              const SizedBox(height: 32),
              Row(
                children: [
                  _TabButton(
                    label: 'Login',
                    active: _isLogin,
                    onTap: () => _switchMode(true),
                  ),
                  _TabButton(
                    label: 'Register',
                    active: !_isLogin,
                    onTap: () => _switchMode(false),
                  ),
                ],
              ),
              const Divider(height: 1),
              const SizedBox(height: 24),
              if (!_isLogin) ...[
                _FieldLabel('Username'),
                TextField(
                  controller: _usernameController,
                  maxLength: 50,
                  decoration: _inputDecoration('yourname'),
                ),
                const SizedBox(height: 12),
              ],
              _FieldLabel('Email'),
              TextField(
                controller: _emailController,
                keyboardType: TextInputType.emailAddress,
                decoration: _inputDecoration('you@email.com'),
              ),
              const SizedBox(height: 12),
              _FieldLabel('Password'),
              TextField(
                controller: _passwordController,
                obscureText: true,
                decoration: _inputDecoration('••••••••'),
              ),
              if (!_isLogin) ...[
                const SizedBox(height: 12),
                _FieldLabel('Confirm Password'),
                TextField(
                  controller: _confirmPasswordController,
                  obscureText: true,
                  decoration: _inputDecoration('••••••••'),
                ),
              ],
              if (_isLogin) ...[
                const SizedBox(height: 8),
                Align(
                  alignment: Alignment.centerRight,
                  child: TextButton(
                    onPressed: () {},
                    child: Text(
                      'Forgot password?',
                      style: TextStyle(fontSize: 12, color: Colors.grey[400]),
                    ),
                  ),
                ),
              ],
              if (_errorMessage != null) ...[
                const SizedBox(height: 8),
                Text(
                  _errorMessage!,
                  style: const TextStyle(fontSize: 12, color: Colors.red),
                ),
              ],
              const SizedBox(height: 20),
              SizedBox(
                width: double.infinity,
                child: FilledButton(
                  onPressed: _submit,
                  style: FilledButton.styleFrom(
                    backgroundColor: Colors.black,
                    padding: const EdgeInsets.symmetric(vertical: 14),
                    shape: RoundedRectangleBorder(
                      borderRadius: BorderRadiusGeometry.circular(8),
                    ),
                  ),
                  child: Text(_isLogin ? 'Login' : 'Create account'),
                ),
              ),
              const SizedBox(height: 16),
              Center(
                child: TextButton(
                  onPressed: () => _switchMode(!_isLogin),
                  child: Text(
                    _isLogin
                        ? 'Don\'t have an account? Register'
                        : 'Already have an account? Login',
                    style: TextStyle(fontSize: 12, color: Colors.grey[600]),
                  ),
                ),
              ),
            ],
          ),
        ),
      ),
    );
  }

  InputDecoration? _inputDecoration(String hint) => InputDecoration(
    hintText: hint,
    hintStyle: TextStyle(color: Colors.grey[300]),
    contentPadding: const EdgeInsets.symmetric(horizontal: 12, vertical: 10),
    border: OutlineInputBorder(borderRadius: BorderRadius.circular(8)),
    counterText: '',
  );
}

class _TabButton extends StatelessWidget {
  final String label;
  final bool active;
  final VoidCallback onTap;

  const _TabButton({
    required this.label,
    required this.active,
    required this.onTap,
  });

  @override
  Widget build(BuildContext context) {
    return Expanded(
      child: GestureDetector(
        onTap: onTap,
        child: Container(
          padding: const EdgeInsets.only(bottom: 10),
          decoration: BoxDecoration(
            border: Border(
              bottom: BorderSide(
                color: active ? Colors.black : Colors.transparent,
                width: 2,
              ),
            ),
          ),
          child: Text(
            label,
            textAlign: TextAlign.center,
            style: TextStyle(
              fontSize: 13,
              color: active ? Colors.black : Colors.grey[400],
            ),
          ),
        ),
      ),
    );
  }
}

class _FieldLabel extends StatelessWidget {
  final String text;

  const _FieldLabel(this.text);

  @override
  Widget build(BuildContext context) {
    return Padding(
      padding: const EdgeInsets.only(bottom: 4),
      child: Text(
        text,
        style: TextStyle(fontSize: 11, color: Colors.grey[600]),
      ),
    );
  }
}
