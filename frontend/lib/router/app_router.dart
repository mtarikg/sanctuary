import 'package:flutter/material.dart';
import 'package:go_router/go_router.dart';
import 'package:riverpod_annotation/riverpod_annotation.dart';
import '../main.dart';
import '../features/notes/models/note.dart';
import '../providers/auth_provider.dart';
import '../features/auth/screens/auth_screen.dart';
import '../features/notes/screens/favorites_screen.dart';
import '../features/notes/screens/home_screen.dart';
import '../features/notes/screens/note_detail_screen.dart';
import '../features/notes/screens/note_form_screen.dart';
import '../features/notes/screens/search_screen.dart';
import '../features/settings/screens/settings_screen.dart';

part 'app_router.g.dart';

final _rootNavigatorKey = GlobalKey<NavigatorState>();
final _shellNavigatorKey = GlobalKey<NavigatorState>();

@riverpod
GoRouter appRouter(AppRouterRef ref) {
  final token = ref.watch(authTokenProvider);

  return GoRouter(
    navigatorKey: _rootNavigatorKey,
    initialLocation: '/home',
    redirect: (context, state) {
      final isAuth = token != null;
      final isOnAuth = state.matchedLocation == '/auth';

      if (!isAuth && !isOnAuth) return '/auth';
      if (isAuth && isOnAuth) return '/home';
      return null;
    },
    routes: [
      GoRoute(path: '/auth', builder: (context, state) => const AuthScreen()),
      StatefulShellRoute.indexedStack(
        builder: (context, state, navigationShell) =>
            MainShell(navigationShell: navigationShell),
        branches: [
          StatefulShellBranch(
            navigatorKey: _shellNavigatorKey,
            routes: [
              GoRoute(
                path: '/home',
                builder: (context, state) => const HomeScreen(),
                routes: [
                  GoRoute(
                    path: 'note/add',
                    builder: (context, state) => const NoteFormScreen(),
                  ),
                  GoRoute(
                    path: 'note/:id',
                    builder: (context, state) {
                      final note = state.extra as Note;
                      return NoteDetailScreen(note: note);
                    },
                  ),
                ],
              ),
            ],
          ),
          StatefulShellBranch(
            routes: [
              GoRoute(
                path: '/search',
                builder: (context, state) => const SearchScreen(),
              ),
            ],
          ),
          StatefulShellBranch(
            routes: [
              GoRoute(
                path: '/favorites',
                builder: (context, state) => const FavoritesScreen(),
              ),
            ],
          ),
        ],
      ),
      GoRoute(
        path: '/settings',
        builder: (context, state) => const SettingsScreen(),
      ),
    ],
  );
}
