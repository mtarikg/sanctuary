import 'package:riverpod_annotation/riverpod_annotation.dart';
import '../../../providers/auth_provider.dart';
import '../services/note_service.dart';
import '../models/category.dart';
import '../models/note.dart';

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

  @override
  bool operator ==(Object other) => false;

  @override
  int get hashCode => Object.hash(categories, notes, loading, error);
}

@riverpod
class NoteNotifier extends _$NoteNotifier {
  late NoteService _noteService;

  @override
  NoteState build() {
    final token = ref.read(authTokenProvider) ?? '';
    _noteService = NoteService(token);
    Future.microtask(() => fetchAll());
    return const NoteState();
  }

  Future<void> fetchAll() async {
    state = state.copyWith(loading: true, error: null);

    try {
      final results = await Future.wait([
        _noteService.fetchCategories(),
        _noteService.fetchNotes(),
      ]);

      final categories = results[0] as List<Category>;
      final notes = results[1] as List<Note>;

      _sortNotes(notes);

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
    final note = await _noteService.addNote(
      title,
      body,
      categoryId: categoryId,
    );

    final updated = [...state.notes, note];
    _sortNotes(updated);
    state = state.copyWith(notes: updated);
  }

  Future<void> updateNote(
    String id,
    String title,
    String body, {
    int? categoryId,
  }) async {
    final updated = await _noteService.updateNote(
      id,
      title,
      body,
      categoryId: categoryId,
    );
    final notes = [...state.notes];
    final index = notes.indexWhere((n) => n.id == id);

    if (index != -1) {
      notes[index] = updated;
      _sortNotes(notes);
      state = state.copyWith(notes: notes);
    }
  }

  Future<void> deleteNote(String id) async {
    await _noteService.deleteNote(id);
    final notes = state.notes.where((n) => n.id != id).toList();
    state = state.copyWith(notes: notes);
  }

  void _sortNotes(List<Note> notes) {
    notes.sort((a, b) {
      final aDate = a.updatedAt ?? a.createdAt;
      final bDate = b.updatedAt ?? b.createdAt;
      return bDate.compareTo(aDate);
    });
  }
}
