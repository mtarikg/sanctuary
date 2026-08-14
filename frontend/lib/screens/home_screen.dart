import 'package:flutter/material.dart';
import 'package:frontend/models/note.dart';
import 'package:frontend/notifiers/note_notifier.dart';
import 'package:frontend/screens/note_detail_screen.dart';
import 'package:frontend/screens/note_form_screen.dart';

class HomeScreen extends StatelessWidget {
  final NoteNotifier notifier;

  const HomeScreen({super.key, required this.notifier});

  @override
  Widget build(BuildContext context) {
    return ListenableBuilder(
      listenable: notifier,
      builder: (context, _) {
        return Scaffold(
          appBar: AppBar(
            title: const Text('Sanctuary.'),
            actions: [
              Padding(
                padding: const EdgeInsets.symmetric(horizontal: 8, vertical: 8),
                child: FloatingActionButton.small(
                  onPressed: () => Navigator.push(
                    context,
                    MaterialPageRoute(
                      builder: (_) => NoteFormScreen(notifier: notifier),
                    ),
                  ),
                  child: const Icon(Icons.add),
                ),
              ),
            ],
          ),
          body: notifier.loading
              ? const Center(child: CircularProgressIndicator())
              : notifier.error != null
              ? Center(child: Text(notifier.error!))
              : notifier.notes.isEmpty
              ? const Center(
                  child: Text(
                    'No notes yet.',
                    style: TextStyle(color: Colors.grey),
                  ),
                )
              : ListView.builder(
                  padding: const EdgeInsets.all(12),
                  itemCount: notifier.notes.length,
                  itemBuilder: (context, index) =>
                      NoteCard(note: notifier.notes[index], notifier: notifier),
                ),
        );
      },
    );
  }
}

class NoteCard extends StatelessWidget {
  final Note note;
  final NoteNotifier notifier;

  const NoteCard({super.key, required this.note, required this.notifier});

  @override
  Widget build(BuildContext context) {
    final displayDate = note.updatedAt ?? note.createdAt;
    final date = '${displayDate.day} ${_month(displayDate.month)}';

    return Card(
      margin: const EdgeInsets.only(bottom: 8),
      shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(12)),
      elevation: 0,
      color: Colors.white,
      child: InkWell(
        borderRadius: BorderRadius.circular(12),
        onTap: () => Navigator.push(
          context,
          MaterialPageRoute(
            builder: (_) => NoteDetailScreen(note: note, notifier: notifier),
          ),
        ),
        child: Padding(
          padding: const EdgeInsets.symmetric(horizontal: 14, vertical: 10),
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              Row(
                mainAxisAlignment: MainAxisAlignment.spaceBetween,
                children: [
                  Expanded(
                    child: Text(
                      note.title,
                      style: const TextStyle(
                        fontSize: 14,
                        fontWeight: FontWeight.w500,
                      ),
                      overflow: TextOverflow.ellipsis,
                    ),
                  ),
                  const SizedBox(width: 8),
                  Text(
                    date,
                    style: TextStyle(fontSize: 11, color: Colors.grey[400]),
                  ),
                ],
              ),
              const SizedBox(height: 3),
              Text(
                note.body,
                style: TextStyle(fontSize: 12, color: Colors.grey[500]),
                maxLines: 1,
                overflow: TextOverflow.ellipsis,
              ),
            ],
          ),
        ),
      ),
    );
  }

  String _month(int m) => [
    'Jan',
    'Feb',
    'Mar',
    'Apr',
    'May',
    'Jun',
    'Jul',
    'Aug',
    'Sep',
    'Oct',
    'Nov',
    'Dec',
  ][m - 1];
}
