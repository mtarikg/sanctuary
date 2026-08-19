import 'dart:convert';
import 'package:http/http.dart' as http;
import '../../../core/constants/constants.dart';
import '../models/category.dart';
import '../models/note.dart';

class NoteService {
  final String token;

  NoteService(this.token);

  Map<String, String> get _headers => {
    'Content-Type': 'application/json',
    'Authorization': 'Bearer $token',
  };

  Future<List<Category>> fetchCategories() async {
    final response = await http.get(
      Uri.parse('${AppConstants.baseUrl}/categories'),
    );
    if (response.statusCode == 200) {
      return (jsonDecode(response.body) as List<dynamic>)
          .map((e) => Category.fromJson(e as Map<String, dynamic>))
          .toList();
    }
    throw Exception('Failed to fetch categories: ${response.statusCode}');
  }

  Future<List<Note>> fetchNotes() async {
    final response = await http.get(
      Uri.parse('${AppConstants.baseUrl}/notes'),
      headers: _headers,
    );

    if (response.statusCode == 200) {
      return (jsonDecode(response.body) as List<dynamic>)
          .map((e) => Note.fromJson(e as Map<String, dynamic>))
          .toList();
    }
    throw Exception('Failed to fetch notes: ${response.statusCode}');
  }

  Future<Note> addNote(String title, String body, {int? categoryId}) async {
    final requestBody = {
      'title': title,
      'body': body,
      'categoryId': ?categoryId,
    };

    final response = await http.post(
      Uri.parse('${AppConstants.baseUrl}/notes'),
      headers: _headers,
      body: jsonEncode(requestBody),
    );

    if (response.statusCode == 201) {
      return Note.fromJson(jsonDecode(response.body));
    }

    throw Exception('Failed to add note: ${response.statusCode}');
  }

  Future<Note> updateNote(
    String id,
    String title,
    String body, {
    int? categoryId,
  }) async {
    final requestBody = {
      'title': title,
      'body': body,
      'categoryId': ?categoryId,
    };

    final response = await http.put(
      Uri.parse('${AppConstants.baseUrl}/notes/$id'),
      headers: _headers,
      body: jsonEncode(requestBody),
    );

    if (response.statusCode == 200) {
      return Note.fromJson(jsonDecode(response.body));
    }

    throw Exception('Failed to update note: ${response.statusCode}');
  }

  Future<void> deleteNote(String id) async {
    final response = await http.delete(
      Uri.parse('${AppConstants.baseUrl}/notes/$id'),
      headers: _headers,
    );

    if (response.statusCode != 204) {
      throw Exception('Failed to delete note: ${response.statusCode}');
    }
  }
}
