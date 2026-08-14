import 'package:frontend/models/category.dart';

class Note {
  final String id;
  final String title;
  final String body;
  final DateTime createdAt;
  final DateTime? updatedAt;
  final Category? category;

  Note({
    required this.id,
    required this.title,
    required this.body,
    required this.createdAt,
    required this.updatedAt,
    required this.category,
  });

  factory Note.fromJson(Map<String, dynamic> json) {
    return Note(
      id: json['id'].toString(),
      title: json['title'],
      body: json['body'],
      createdAt: DateTime.parse(json['createdAt']),
      updatedAt: json['updatedAt'] != null
          ? DateTime.parse(json['updatedAt'])
          : null,
      category: json['category'] != null
          ? Category.fromJson(json['category'])
          : null,
    );
  }
}
