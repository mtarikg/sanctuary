import 'package:flutter/material.dart';
import 'package:flutter_secure_storage/flutter_secure_storage.dart';
import 'package:frontend/notifiers/note_notifier.dart';
import 'package:frontend/screens/auth_screen.dart';
import 'package:frontend/screens/favorites_screen.dart';
import 'package:frontend/screens/search_screen.dart';
import 'screens/home_screen.dart';

final _storage = FlutterSecureStorage();

void main() async {
  WidgetsFlutterBinding.ensureInitialized();
  final token = await _storage.read(key: 'auth_token');
  if (token != null) authToken = token;
  runApp(SanctuaryApp(isAuthenticated: token != null));
}

class SanctuaryApp extends StatelessWidget {
  final bool isAuthenticated;

  const SanctuaryApp({super.key, required this.isAuthenticated});

  @override
  Widget build(BuildContext context) {
    return MaterialApp(
      title: 'sanctuary-app',
      debugShowCheckedModeBanner: false,
      theme: ThemeData(
        colorScheme: .fromSeed(seedColor: Colors.lightBlue),
        appBarTheme: const AppBarTheme(centerTitle: true),
      ),
      home: isAuthenticated ? const MainShell() : const AuthScreen(),
    );
  }
}

class MainShell extends StatefulWidget {
  const MainShell({super.key});

  @override
  State<MainShell> createState() => _MainShellState();
}

class _MainShellState extends State<MainShell> {
  int _currentIndex = 0;
  final NoteNotifier _noteNotifier = NoteNotifier();

  @override
  void initState() {
    super.initState();
    _noteNotifier.fetchAll();
  }

  @override
  void dispose() {
    _noteNotifier.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    final List<Widget> screens = [
      HomeScreen(notifier: _noteNotifier),
      SearchScreen(notifier: _noteNotifier),
      const FavoritesScreen(),
    ];

    return Scaffold(
      body: screens[_currentIndex],
      bottomNavigationBar: NavigationBar(
        selectedIndex: _currentIndex,
        onDestinationSelected: (index) => setState(() => _currentIndex = index),
        destinations: const [
          NavigationDestination(
            icon: Icon(Icons.home_outlined),
            selectedIcon: Icon(Icons.home),
            label: 'Home',
          ),
          NavigationDestination(icon: Icon(Icons.search), label: 'Search'),
          NavigationDestination(
            icon: Icon(Icons.star_outline),
            selectedIcon: Icon(Icons.star),
            label: 'Favorites',
          ),
        ],
      ),
    );
  }
}
