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
  bool _isLoading = false;

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

    setState(() => _isLoading = true);

    try {
      await ref
          .read(noteNotifierProvider.notifier)
          .addNote(
            _titleController.text.trim(),
            _bodyController.text.trim(),
            categoryId: _selectedCategory?.id,
          );

      if (mounted) context.pop();
    } catch (e) {
      setState(() {
        _error = 'Failed to add note. Try again.';
        _isLoading = false;
      });
    }
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
              onPressed: _isLoading ? null : _submit,
              style: FilledButton.styleFrom(
                backgroundColor: Theme.of(context).colorScheme.primary,
                shape: RoundedRectangleBorder(
                  borderRadius: BorderRadius.circular(8),
                ),
              ),
              child: _isLoading
                  ? const SizedBox(
                      height: 16,
                      width: 16,
                      child: CircularProgressIndicator(
                        strokeWidth: 2,
                        color: Colors.white,
                      ),
                    )
                  : const Text('Add'),
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
