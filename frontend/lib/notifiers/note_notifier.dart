import 'dart:convert';
import 'package:flutter/material.dart';
import 'package:http/http.dart' as http;
import '../models/category.dart';
import '../models/note.dart';
import '../screens/auth_screen.dart';

class NoteNotifier extends ChangeNotifier {
  List<Category> categories = [];
  List<Note> notes = [];
  bool loading = false;
  String? error;

  static const String _base = 'http://10.0.2.2:5002';

  Map<String, String> get _headers => {
    'Content-Type': 'application/json',
    'Authorization': 'Bearer $authToken',
  };

  Future<void> fetchAll() async {
    loading = true;
    error = null;
    notifyListeners();

    try {
      final results = await Future.wait([
        http.get(Uri.parse('$_base/categories')),
        http.get(Uri.parse('$_base/notes'), headers: _headers),
      ]);

      final categoriesResult = results[0];
      final notesResult = results[1];

      if (categoriesResult.statusCode == 200) {
        final List<dynamic> data = jsonDecode(categoriesResult.body);
        categories = data.map((e) => Category.fromJson(e)).toList();
      }

      if (notesResult.statusCode == 200) {
        final List<dynamic> data = jsonDecode(notesResult.body);
        notes = data.map((e) => Note.fromJson(e)).toList();
        _sortNotes();
      } else {
        error = 'Failed to load notes.';
      }
    } catch (e) {
      error = 'Could not reach the server.';
    }

    loading = false;
    notifyListeners();
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
        notes.insert(0, note);
        _sortNotes();
        notifyListeners();
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
        'categoryId': categoryId,
      };

      final response = await http.put(
        Uri.parse('$_base/notes/$id'),
        headers: _headers,
        body: jsonEncode(requestBody),
      );

      if (response.statusCode == 200) {
        final updated = Note.fromJson(jsonDecode(response.body));
        final index = notes.indexWhere((n) => n.id == id);

        if (index != -1) {
          notes[index] = updated;
          _sortNotes();
          notifyListeners();
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
        notes.removeWhere((n) => n.id == id);
        notifyListeners();
      }
    } catch (e) {
      print('deleteNote error: $e');
    }
  }

  void _sortNotes() {
    notes.sort((a, b) {
      final aDate = a.updatedAt ?? a.createdAt;
      final bDate = b.updatedAt ?? b.createdAt;
      return bDate.compareTo(aDate);
    });
  }
}
