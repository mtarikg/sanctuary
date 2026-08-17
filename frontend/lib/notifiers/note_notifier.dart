import 'dart:convert';
import 'package:http/http.dart' as http;
import 'package:riverpod_annotation/riverpod_annotation.dart';
import '../models/category.dart';
import '../models/note.dart';
import '../screens/auth_screen.dart';

part 'note_notifier.g.dart';

class NoteState {
  final List<Category> categories;
  final List<Note> notes;
  final bool loading;
  final String? error;

  const NoteState({
    this.categories = const [],
    this.notes = const [],
    this.loading = false,
    this.error,
  });

  NoteState copyWith({
    List<Category>? categories,
    List<Note>? notes,
    bool? loading,
    String? error,
  }) {
    return NoteState(
      categories: categories ?? this.categories,
      notes: notes ?? this.notes,
      loading: loading ?? this.loading,
      error: error ?? this.error,
    );
  }
}

@riverpod
class NoteNotifier extends _$NoteNotifier {
  static const String _base = 'http://10.0.2.2:5002';

  Map<String, String> get _headers => {
    'Content-Type': 'application/json',
    'Authorization': 'Bearer $authToken',
  };

  @override
  NoteState build() {
    Future.microtask(() => fetchAll());
    return const NoteState();
  }

  Future<void> fetchAll() async {
    state = state.copyWith(loading: true, error: null);

    try {
      final results = await Future.wait([
        http.get(Uri.parse('$_base/categories')),
        http.get(Uri.parse('$_base/notes'), headers: _headers),
      ]);

      final categoriesResult = results[0];
      final notesResult = results[1];

      final categories = categoriesResult.statusCode == 200
          ? (jsonDecode(categoriesResult.body) as List<dynamic>)
                .map((e) => Category.fromJson(e as Map<String, dynamic>))
                .toList()
          : <Category>[];

      final notes = notesResult.statusCode == 200
          ? (jsonDecode(notesResult.body) as List<dynamic>)
                .map((e) => Note.fromJson(e as Map<String, dynamic>))
                .toList()
          : <Note>[];

      _sortNotes(notes);

      if (notesResult.statusCode != 200) {
        state = state.copyWith(loading: false, error: 'Failed to load notes.');
      }

      state = state.copyWith(
        categories: categories,
        notes: notes,
        loading: false,
      );
    } catch (e) {
      state = state.copyWith(
        loading: false,
        error: 'Could not reach the server.',
      );
    }
  }

  Future<void> addNote(String title, String body, {int? categoryId}) async {
    try {
      final requestBody = {
        'title': title,
        'body': body,
        'categoryId': ?categoryId,
      };

      final response = await http.post(
        Uri.parse('$_base/notes'),
        headers: _headers,
        body: jsonEncode(requestBody),
      );

      if (response.statusCode == 201) {
        final note = Note.fromJson(jsonDecode(response.body));
        final updated = [...state.notes, note];
        _sortNotes(updated);
        state = state.copyWith(notes: updated);
      }
    } catch (e) {
      print('addNote error: $e');
      // gonna handle in refactor
    }
  }

  Future<void> updateNote(
    String id,
    String title,
    String body, {
    int? categoryId,
  }) async {
    try {
      final requestBody = {
        'title': title,
        'body': body,
        'categoryId': ?categoryId,
      };

      final response = await http.put(
        Uri.parse('$_base/notes/$id'),
        headers: _headers,
        body: jsonEncode(requestBody),
      );

      if (response.statusCode == 200) {
        final updated = Note.fromJson(jsonDecode(response.body));
        final notes = [...state.notes];
        final index = notes.indexWhere((n) => n.id == id);

        if (index != -1) {
          notes[index] = updated;
          _sortNotes(notes);
          state = state.copyWith(notes: notes);
        }
      }
    } catch (e) {
      print('updateNote error: $e');
    }
  }

  Future<void> deleteNote(String id) async {
    try {
      final response = await http.delete(
        Uri.parse('$_base/notes/$id'),
        headers: _headers,
      );

      if (response.statusCode == 204) {
        final notes = state.notes.where((n) => n.id != id).toList();
        state = state.copyWith(notes: notes);
      }
    } catch (e) {
      print('deleteNote error: $e');
    }
  }

  void _sortNotes(List<Note> notes) {
    notes.sort((a, b) {
      final aDate = a.updatedAt ?? a.createdAt;
      final bDate = b.updatedAt ?? b.createdAt;
      return bDate.compareTo(aDate);
    });
  }
}
