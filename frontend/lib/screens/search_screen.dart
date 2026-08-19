import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:frontend/notifiers/note_notifier.dart';
import 'package:frontend/utils/note_utils.dart';
import 'package:frontend/widgets/note_card.dart';

class SearchScreen extends ConsumerStatefulWidget {
  const SearchScreen({super.key});

  @override
  ConsumerState<SearchScreen> createState() => _SearchScreenState();
}

class _SearchScreenState extends ConsumerState<SearchScreen> {
  final TextEditingController _searchController = TextEditingController();
  String _query = '';

  @override
  void dispose() {
    _searchController.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    final state = ref.watch(noteNotifierProvider);
    final filtered = filterNotes(state.notes, _query);

    return Scaffold(
      appBar: AppBar(title: const Text('Search')),
      body: Column(
        children: [
          Padding(
            padding: const EdgeInsets.fromLTRB(12, 10, 12, 0),
            child: TextField(
              controller: _searchController,
              onChanged: (value) => setState(() => _query = value),
              decoration: InputDecoration(
                hintText: 'Search notes...',
                prefixIcon: const Icon(Icons.search),
                border: OutlineInputBorder(
                  borderRadius: BorderRadius.circular(8),
                ),
                contentPadding: const EdgeInsets.symmetric(
                  horizontal: 12,
                  vertical: 10,
                ),
              ),
            ),
          ),
          const SizedBox(height: 12),
          Expanded(
            child: _query.trim().isEmpty
                ? const Center(
                    child: Text(
                      'Start typing to search your notes',
                      style: TextStyle(color: Colors.grey),
                    ),
                  )
                : filtered.isEmpty
                ? const Center(
                    child: Text(
                      'No notes found.',
                      style: TextStyle(color: Colors.grey),
                    ),
                  )
                : ListView.builder(
                    padding: const EdgeInsets.symmetric(horizontal: 12),
                    itemCount: filtered.length,
                    itemBuilder: ((context, index) =>
                        NoteCard(note: filtered[index])),
                  ),
          ),
        ],
      ),
    );
  }
}
