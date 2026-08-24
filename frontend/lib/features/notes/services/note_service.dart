import 'package:dio/dio.dart';
import 'package:frontend/core/network/dio_client.dart';
import '../models/note.dart';

class NoteService {
  final Dio _dio = DioClient.instance.dio;

  Future<List<Note>> fetchNotes() async {
    final response = await _dio.get('/notes');

    return (response.data as List<dynamic>)
        .map((e) => Note.fromJson(e as Map<String, dynamic>))
        .toList();
  }

  Future<Note> addNote(String title, String body, {int? categoryId}) async {
    final requestBody = {
      'title': title,
      'body': body,
      'categoryId': ?categoryId,
    };

    final response = await _dio.post('/notes', data: requestBody);

    return Note.fromJson(response.data as Map<String, dynamic>);
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

    final response = await _dio.put('/notes/$id', data: requestBody);

    return Note.fromJson(response.data as Map<String, dynamic>);
  }

  Future<void> deleteNote(String id) async {
    await _dio.delete('/notes/$id');
  }
}
