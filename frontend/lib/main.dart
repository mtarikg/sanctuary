import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:flutter_secure_storage/flutter_secure_storage.dart';
import 'package:frontend/constants/constants.dart';
import 'package:frontend/providers/auth_provider.dart';
import 'package:frontend/screens/auth_screen.dart';
import 'package:frontend/screens/favorites_screen.dart';
import 'package:frontend/screens/search_screen.dart';
import 'screens/home_screen.dart';

final navigatorKey = GlobalKey<NavigatorState>();

void main() async {
  WidgetsFlutterBinding.ensureInitialized();
  final token = await const FlutterSecureStorage().read(
    key: AppConstants.authTokenKey,
  );
  runApp(
    ProviderScope(
      overrides: [authTokenProvider.overrideWith((ref) => token)],
      child: const SanctuaryApp(),
    ),
  );
}

class SanctuaryApp extends ConsumerWidget {
  const SanctuaryApp({super.key});

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final token = ref.watch(authTokenProvider);

    ref.listen(authTokenProvider, ((previous, next) {
      if (next == null) {
        navigatorKey.currentState?.pushAndRemoveUntil(
          MaterialPageRoute(builder: (_) => AuthScreen()),
          (_) => false,
        );
      } else if (next != null && previous == null) {
        navigatorKey.currentState?.pushAndRemoveUntil(
          MaterialPageRoute(builder: (_) => MainShell()),
          (_) => false,
        );
      }
    }));

    return MaterialApp(
      title: 'sanctuary-app',
      debugShowCheckedModeBanner: false,
      theme: ThemeData(
        colorScheme: ColorScheme.fromSeed(seedColor: Colors.lightBlue),
        appBarTheme: const AppBarTheme(centerTitle: true),
      ),
      navigatorKey: navigatorKey,
      home: token != null ? const MainShell() : const AuthScreen(),
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

  @override
  Widget build(BuildContext context) {
    final List<Widget> screens = const [
      HomeScreen(),
      SearchScreen(),
      FavoritesScreen(),
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
