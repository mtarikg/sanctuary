import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:frontend/widgets/note_card.dart';
import '../notifiers/note_notifier.dart';
import '../screens/note_form_screen.dart';
import '../screens/settings_screen.dart';

class HomeScreen extends ConsumerWidget {
  const HomeScreen({super.key});

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final state = ref.watch(noteNotifierProvider);

    return Scaffold(
      appBar: AppBar(
        title: const Text('Sanctuary.'),
        actions: [
          Padding(
            padding: const EdgeInsets.symmetric(horizontal: 8, vertical: 8),
            child: FloatingActionButton.small(
              onPressed: () => Navigator.push(
                context,
                MaterialPageRoute(builder: (_) => NoteFormScreen()),
              ),
              child: const Icon(Icons.add),
            ),
          ),
          Padding(
            padding: const EdgeInsets.symmetric(horizontal: 8, vertical: 8),
            child: IconButton(
              onPressed: () => Navigator.push(
                context,
                MaterialPageRoute(builder: (_) => const SettingsScreen()),
              ),
              icon: const Icon(Icons.settings_outlined),
            ),
          ),
        ],
      ),
      body: state.loading
          ? const Center(child: CircularProgressIndicator())
          : state.error != null
          ? Center(child: Text(state.error!))
          : state.notes.isEmpty
          ? const Center(
              child: Text(
                'No notes yet.',
                style: TextStyle(color: Colors.grey),
              ),
            )
          : ListView.builder(
              padding: const EdgeInsets.all(12),
              itemCount: state.notes.length,
              itemBuilder: (context, index) =>
                  NoteCard(note: state.notes[index]),
            ),
    );
  }
}
