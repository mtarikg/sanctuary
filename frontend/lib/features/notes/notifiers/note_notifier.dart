import 'package:freezed_annotation/freezed_annotation.dart';
import 'package:frontend/core/utils/error_mapper.dart';
import 'package:frontend/features/notes/services/category_service.dart';
import 'package:riverpod_annotation/riverpod_annotation.dart';
import '../services/note_service.dart';
import '../models/category.dart';
import '../models/note.dart';

part 'note_notifier.freezed.dart';
part 'note_notifier.g.dart';

@freezed
class NoteState with _$NoteState {
  const factory NoteState({
    @Default([]) List<Category> categories,
    @Default([]) List<Note> notes,
    @Default(false) bool loading,
    String? error,
  }) = _NoteState;
}

@riverpod
class NoteNotifier extends _$NoteNotifier {
  late CategoryService _categoryService;
  late NoteService _noteService;

  @override
  NoteState build() {
    _categoryService = CategoryService();
    _noteService = NoteService();
    Future.microtask(() async {
      await fetchCategories();
      await fetchNotes();
    });
    return const NoteState();
  }

  Future<void> fetchCategories() async {
    state = state.copyWith(loading: true, error: null);

    try {
      final categories = await _categoryService.fetchCategories();
      state = state.copyWith(categories: categories, loading: false);
    } catch (e) {
      state = state.copyWith(loading: false, error: mapError(e));
    }
  }

  Future<void> fetchNotes() async {
    state = state.copyWith(loading: true, error: null);
    try {
      final notes = await _noteService.fetchNotes();
      _sortNotes(notes);
      state = state.copyWith(notes: notes, loading: false);
    } catch (e) {
      state = state.copyWith(loading: false, error: mapError(e));
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
