import 'package:dio/dio.dart';
import 'package:frontend/core/network/dio_client.dart';
import 'package:frontend/features/notes/models/category.dart';

class CategoryService {
  final Dio _dio = DioClient.instance.dio;

  Future<List<Category>> fetchCategories() async {
    final response = await _dio.get('/categories');
    return (response.data as List<dynamic>)
        .map((e) => Category.fromJson(e as Map<String, dynamic>))
        .toList();
  }
}
