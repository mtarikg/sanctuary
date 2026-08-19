import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';
import '../notifiers/note_notifier.dart';
import '../../../core/widgets/note_form_fields.dart';
import '../models/category.dart';

class NoteFormScreen extends ConsumerStatefulWidget {
  const NoteFormScreen({super.key});

  @override
  ConsumerState<NoteFormScreen> createState() => _NoteFormScreenState();
}

class _NoteFormScreenState extends ConsumerState<NoteFormScreen> {
  final _titleController = TextEditingController();
  final _bodyController = TextEditingController();
  Category? _selectedCategory;
  String? _error;

  @override
  void dispose() {
    _titleController.dispose();
    _bodyController.dispose();
    super.dispose();
  }

  Future<void> _submit() async {
    if (_titleController.text.trim().isEmpty) {
      setState(() => _error = 'Title is required.');
      return;
    }

    await ref
        .read(noteNotifierProvider.notifier)
        .addNote(
          _titleController.text.trim(),
          _bodyController.text.trim(),
          categoryId: _selectedCategory?.id,
        );

    if (mounted) context.pop();
  }

  @override
  Widget build(BuildContext context) {
    final categories = ref.read(noteNotifierProvider).categories;

    return Scaffold(
      appBar: AppBar(
        title: const Text('New note'),
        actions: [
          Padding(
            padding: const EdgeInsets.symmetric(horizontal: 8, vertical: 8),
            child: FilledButton(
              onPressed: _submit,
              style: FilledButton.styleFrom(
                backgroundColor: Theme.of(context).colorScheme.primary,
                shape: RoundedRectangleBorder(
                  borderRadius: BorderRadius.circular(8),
                ),
              ),
              child: const Text('Save'),
            ),
          ),
        ],
      ),
      body: SingleChildScrollView(
        padding: const EdgeInsets.all(24),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            NoteFormFields(
              titleController: _titleController,
              bodyController: _bodyController,
              categories: categories,
              selectedCategory: _selectedCategory,
              onCategoryChanged: (val) =>
                  setState(() => _selectedCategory = val),
              error: _error,
            ),
          ],
        ),
      ),
    );
  }
}
