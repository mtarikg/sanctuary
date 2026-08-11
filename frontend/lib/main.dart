import 'package:flutter/material.dart';
import 'package:frontend/screens/auth_screen.dart';
import 'package:frontend/screens/favorites_screen.dart';
import 'package:frontend/screens/search_screen.dart';
import 'screens/home_screen.dart';

void main() {
  runApp(const SanctuaryApp());
}

class SanctuaryApp extends StatelessWidget {
  const SanctuaryApp({super.key});

  @override
  Widget build(BuildContext context) {
    return MaterialApp(
      title: 'sanctuary-app',
      debugShowCheckedModeBanner: false,
      theme: ThemeData(
        colorScheme: .fromSeed(seedColor: Colors.lightBlue),
        appBarTheme: const AppBarTheme(centerTitle: true),
      ),
      home: const AuthScreen(),
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

  final List<Widget> _screens = const [
    HomeScreen(),
    SearchScreen(),
    FavoritesScreen(),
  ];

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      body: _screens[_currentIndex],
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
