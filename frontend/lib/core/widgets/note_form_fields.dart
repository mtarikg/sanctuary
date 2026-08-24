import 'package:flutter/material.dart';
import '../../features/notes/models/category.dart';

class NoteFormFields extends StatelessWidget {
  final TextEditingController titleController;
  final TextEditingController bodyController;
  final List<Category> categories;
  final Category? selectedCategory;
  final ValueChanged<Category?> onCategoryChanged;
  final String? error;

  const NoteFormFields({
    super.key,
    required this.titleController,
    required this.bodyController,
    required this.categories,
    required this.selectedCategory,
    required this.onCategoryChanged,
    this.error,
  });

  @override
  Widget build(BuildContext context) {
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        DropdownButtonFormField<Category>(
          initialValue: selectedCategory,
          hint: const Text('Category (optional)'),
          items: categories
              .map((c) => DropdownMenuItem(value: c, child: Text(c.name)))
              .toList(),
          onChanged: onCategoryChanged,
          decoration: InputDecoration(
            border: OutlineInputBorder(borderRadius: BorderRadius.circular(8)),
            contentPadding: const EdgeInsets.symmetric(
              horizontal: 12,
              vertical: 10,
            ),
          ),
        ),
        const SizedBox(height: 12),
        TextField(
          controller: titleController,
          decoration: InputDecoration(
            hintText: 'Title',
            border: OutlineInputBorder(borderRadius: BorderRadius.circular(8)),
            contentPadding: const EdgeInsets.symmetric(
              horizontal: 12,
              vertical: 10,
            ),
          ),
        ),
        const SizedBox(height: 12),
        TextField(
          controller: bodyController,
          maxLines: null,
          minLines: 6,
          keyboardType: TextInputType.multiline,
          decoration: InputDecoration(
            hintText: 'Write something...',
            border: OutlineInputBorder(borderRadius: BorderRadius.circular(8)),
            contentPadding: const EdgeInsets.symmetric(
              horizontal: 12,
              vertical: 10,
            ),
          ),
        ),
        if (error != null) ...[
          const SizedBox(height: 8),
          Text(error!, style: const TextStyle(fontSize: 12, color: Colors.red)),
        ],
      ],
    );
  }
}
