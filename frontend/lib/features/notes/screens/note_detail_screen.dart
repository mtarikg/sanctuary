import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';
import '../models/category.dart';
import '../models/note.dart';
import '../notifiers/note_notifier.dart';
import '../../../core/widgets/note_form_fields.dart';

class NoteDetailScreen extends ConsumerStatefulWidget {
  final Note note;

  const NoteDetailScreen({super.key, required this.note});

  @override
  ConsumerState<NoteDetailScreen> createState() => _NoteDetailScreenState();
}

class _NoteDetailScreenState extends ConsumerState<NoteDetailScreen> {
  bool _editing = false;
  Category? _selectedCategory;
  late TextEditingController _titleController;
  late TextEditingController _bodyController;
  String? _error;

  @override
  void initState() {
    super.initState();
    _titleController = TextEditingController(text: widget.note.title);
    _bodyController = TextEditingController(text: widget.note.body);
    _selectedCategory = widget.note.category;
  }

  @override
  void dispose() {
    _titleController.dispose();
    _bodyController.dispose();
    super.dispose();
  }

  Future<void> _save() async {
    if (_titleController.text.trim().isEmpty) {
      setState(() => _error = 'Title is required.');
      return;
    }

    if (_bodyController.text.trim().isEmpty) {
      setState(() => _error = 'Body is required.');
      return;
    }

    await ref
        .read(noteNotifierProvider.notifier)
        .updateNote(
          widget.note.id,
          _titleController.text.trim(),
          _bodyController.text.trim(),
          categoryId: _selectedCategory?.id,
        );

    if (mounted) context.pop();
  }

  Future<void> _delete() async {
    final confirm = await showDialog<bool>(
      context: context,
      builder: (dialogContext) => AlertDialog(
        title: const Text('Delete Note'),
        content: const Text('This cannot be undone.'),
        actions: [
          TextButton(
            onPressed: () => Navigator.of(dialogContext).pop(false),
            child: const Text('Cancel'),
          ),
          TextButton(
            onPressed: () => Navigator.of(dialogContext).pop(true),
            child: const Text('Delete', style: TextStyle(color: Colors.red)),
          ),
        ],
      ),
    );

    if (confirm == true && mounted) {
      await ref.read(noteNotifierProvider.notifier).deleteNote(widget.note.id);
      if (mounted) context.pop();
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: Text(_editing ? 'Edit note' : 'Note'),
        actions: [
          if (!_editing) ...[
            IconButton(
              icon: const Icon(Icons.edit_outlined),
              onPressed: () => setState(() => _editing = true),
            ),
            IconButton(
              icon: const Icon(Icons.delete_outline),
              onPressed: _delete,
            ),
          ] else
            Padding(
              padding: const EdgeInsets.symmetric(horizontal: 8, vertical: 8),
              child: FilledButton(
                onPressed: _save,
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
        child: _editing ? _editView() : readView(),
      ),
    );
  }

  Widget readView() {
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Text(
          widget.note.title,
          style: const TextStyle(fontSize: 20, fontWeight: FontWeight.w500),
        ),
        const SizedBox(height: 4),
        Text(
          _selectedCategory != null ? _selectedCategory!.name : 'Uncategorized',
          style: TextStyle(fontSize: 12, color: Colors.grey[400]),
        ),
        const SizedBox(height: 16),
        Text(
          widget.note.body,
          style: const TextStyle(fontSize: 14, height: 1.6),
        ),
      ],
    );
  }

  Widget _editView() {
    final categories = ref.watch(noteNotifierProvider).categories;

    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        NoteFormFields(
          titleController: _titleController,
          bodyController: _bodyController,
          categories: categories,
          selectedCategory: _selectedCategory,
          onCategoryChanged: (val) => setState(() => _selectedCategory = val),
          error: _error,
        ),
      ],
    );
  }
}
