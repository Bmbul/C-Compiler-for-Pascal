# C# compiler
MCS = mcs

# Output executable
OUT_EXEC = parser.exe

# Directories
COMMON_DIR = Compiler.Common
LEXICAL_ANALYSER_DIR = Compiler.LexicalAnalyser
PARSER_DIR = Compiler.Parser
GENERATOR_DIR = Compiler.Generator

# Source files using find
COMMON_SOURCES = $(shell find $(COMMON_DIR) -name '*.cs')
LEXICAL_SOURCES = $(shell find $(LEXICAL_ANALYSER_DIR) -name '*.cs')
PARSER_SOURCES = $(shell find $(PARSER_DIR) -name '*.cs')
GENERATOR_SOURCES = $(shell find $(GENERATOR_DIR) -name '*.cs')

# Compiled DLL paths
COMMON_DLL = $(COMMON_DIR)/Compiler.Common.dll
GENERATOR_DLL = $(GENERATOR_DIR)/Compiler.Generator.dll
PARSER_DLL = $(PARSER_DIR)/Compiler.Parser.dll

# Build all targets
all: $(OUT_EXEC)

# Build the common library
$(COMMON_DLL): $(COMMON_SOURCES)
	$(MCS) -target:library -out:$@ $^

# Build the generator library (depends on common)
$(GENERATOR_DLL): $(GENERATOR_SOURCES) $(COMMON_DLL)
	$(MCS) -target:library -out:$@ $(GENERATOR_SOURCES) -r:$(COMMON_DLL)

# Build the parser library (depends on common and generator)
$(PARSER_DLL): $(PARSER_SOURCES) $(COMMON_DLL) $(GENERATOR_DLL)
	$(MCS) -target:library -out:$@ $(PARSER_SOURCES) -r:$(COMMON_DLL) -r:$(GENERATOR_DLL)

# Build the main executable (depends on parser and lexical analyzer)
$(OUT_EXEC): $(LEXICAL_SOURCES) $(PARSER_DLL) $(COMMON_DLL) $(GENERATOR_DLL)
	$(MCS) -out:$@ $(LEXICAL_SOURCES) -r:$(COMMON_DLL) -r:$(GENERATOR_DLL) -r:$(PARSER_DLL)

# Clean up built files
clean:
	rm -rf $(OUT_EXEC) $(COMMON_DLL) $(GENERATOR_DLL) $(PARSER_DLL)

.PHONY: all clean
