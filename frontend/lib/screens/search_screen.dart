import 'package:flutter/material.dart';
import 'package:frontend/notifiers/note_notifier.dart';
import 'package:frontend/screens/home_screen.dart';

class SearchScreen extends StatefulWidget {
  final NoteNotifier notifier;

  const SearchScreen({super.key, required this.notifier});

  @override
  State<SearchScreen> createState() => _SearchScreenState();
}

class _SearchScreenState extends State<SearchScreen> {
  final TextEditingController _searchController = TextEditingController();
  String _query = '';

  List<dynamic> get _filtered {
    if (_query.trim().isEmpty) return [];

    final q = _query.toLowerCase();
    return widget.notifier.notes
        .where(
          (n) =>
              n.title.toLowerCase().contains(q) ||
              n.body.toLowerCase().contains(q),
        )
        .toList();
  }

  @override
  void dispose() {
    _searchController.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
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
                : _filtered.isEmpty
                ? const Center(
                    child: Text(
                      'No notes found.',
                      style: TextStyle(color: Colors.grey),
                    ),
                  )
                : ListView.builder(
                    padding: const EdgeInsets.symmetric(horizontal: 12),
                    itemCount: _filtered.length,
                    itemBuilder: ((context, index) =>
                        NoteCard(note: _filtered[index])),
                  ),
          ),
        ],
      ),
    );
  }
}
