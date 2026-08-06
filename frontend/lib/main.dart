import 'package:flutter/material.dart';
import 'package:http/http.dart' as http;

void main() {
  runApp(const MyApp());
}

class MyApp extends StatelessWidget {
  const MyApp({super.key});

  @override
  Widget build(BuildContext context) {
    return MaterialApp(
      title: 'sanctuary-app',
      theme: ThemeData(
        colorScheme: .fromSeed(seedColor: Colors.lightBlue),
        appBarTheme: const AppBarTheme(centerTitle: true),
      ),
      home: const MyHomePage(title: 'Sanctuary.'),
    );
  }
}

class MyHomePage extends StatefulWidget {
  const MyHomePage({super.key, required this.title});

  final String title;

  @override
  State<MyHomePage> createState() => _MyHomePageState();
}

class _MyHomePageState extends State<MyHomePage> {
  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        backgroundColor: Theme.of(context).colorScheme.inversePrimary,
        title: Text(widget.title),
      ),
      body: Center(
        child: Column(
          mainAxisAlignment: .center,
          children: [
            const Text('Your sanctuary place for all your thoughts.'),
            Text(_result),
          ],
        ),
      ),
      floatingActionButton: Row(
        children: [
          FloatingActionButton(
            onPressed: _fetchNotes,
            child: const Icon(Icons.add),
          ),
          SizedBox(width: 15, height: 15),
          FloatingActionButton(
            onPressed: () => _deleteNoteById(9),
            child: const Icon(Icons.delete),
          ),
        ],
      ),
      floatingActionButtonLocation: FloatingActionButtonLocation.endTop,
    );
  }

  String _result = '';

  Future<void> _fetchNotes() async {
    final response = await http.get(Uri.parse('http://10.0.2.2:5002/notes'));

    setState(() {
      _result = 'Status: ${response.statusCode}\n${response.body}';
    });
  }

  Future<void> _deleteNoteById(int id) async {
    final response = await http.delete(
      Uri.parse('http://10.0.2.2:5002/notes/$id'),
    );

    setState(() {
      _result = 'Status: ${response.statusCode}\n${response.body}';
    });
  }
}
