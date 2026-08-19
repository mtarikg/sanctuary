import '../../features/notes/models/note.dart';

List<Note> filterNotes(List<Note> notes, String query) {
  final q = query.toLowerCase().trim();
  if (q.isEmpty) return [];

  return notes
      .where(
        (n) =>
            n.title.toLowerCase().contains(q) ||
            n.body.toLowerCase().contains(q),
      )
      .toList();
}
