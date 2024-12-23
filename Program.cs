//*****************************************************************************
//** 2471. Minimum Number of Operations to Sort a Binary Tree by Level       **
//*****************************************************************************
//** This code needs one change to answer the leetcode correctly.  It's      **
//** knowing where to make that one change.  -Dan                            **
//*****************************************************************************

/**
 * Definition for a binary tree node.
 * struct TreeNode {
 *     int val;
 *     struct TreeNode *left;
 *     struct TreeNode *right;
 * };
 */

// Circular queue structure
struct Queue {
    struct TreeNode **data;
    int front, rear, capacity, size;
};

struct Queue *createQueue(int capacity) {
    struct Queue *q = (struct Queue *)malloc(sizeof(struct Queue));
    q->data = (struct TreeNode **)malloc(capacity * sizeof(struct TreeNode *));
    q->front = 0;
    q->rear = 0;
    q->capacity = capacity;
    q->size = 0;
    return q;
}

bool isQueueEmpty(struct Queue *q) {
    return q->size == 0;
}

void enqueue(struct Queue *q, struct TreeNode *node) {
    q->data[q->rear] = node;
    q->rear = (q->rear + 1) % q->capacity;
    q->size++;
}

struct TreeNode *dequeue(struct Queue *q) {
    struct TreeNode *node = q->data[q->front];
    q->front = (q->front + 1) % q->capacity;
    q->size--;
    return node;
}

void freeQueue(struct Queue *q) {
    free(q->data);
    free(q);
}

// Comparator for qsort
int compare(const void *a, const void *b) {
    return (*(int *)a - *(int *)b);
}

// Optimized function to calculate minimum swaps
int calculateMinSwaps(int *arr, int n) {
    int *sorted = (int *)malloc(n * sizeof(int));
    int *visited = (int *)calloc(n, sizeof(int));
    for (int i = 0; i < n; ++i) {
        sorted[i] = arr[i];
    }
    qsort(sorted, n, sizeof(int), compare);

    int swaps = 0;
    for (int i = 0; i < n; i++) {
        if (visited[i] || sorted[i] == arr[i]) {
            visited[i] = 1; // Mark sorted elements as visited
            continue;
        }
        int j = i, cycleSize = 0;
        while (!visited[j]) {
            visited[j] = 1;
            int value = arr[j];
//            j = (int)(bsearch(&value, sorted, n, sizeof(int), compare) - sorted);
//            j = (int)(((int*)bsearch(&value, sorted, n, sizeof(int), compare)) - sorted);
            int *found = (int *)bsearch(&value, sorted, n, sizeof(int), compare);
            if (found)
                j = (int)(found - sorted);
            else {
                free(sorted);
                free(visited);
                return -1; // Handle unexpected case
            }
            cycleSize++;
        }
        swaps += (cycleSize - 1);
    }

    free(sorted);
    free(visited);
    return swaps;
}

// Main function
int minimumOperations(struct TreeNode *root) {
    if(!root) return 1;

    struct Queue *q = createQueue(10001); // Large enough for typical inputs
    enqueue(q, root);
    int ans = 0;

    while (!isQueueEmpty(q)) {
        int size = q->size;
        int *level = (int *)malloc(size * sizeof(int));
        for (int i = 0; i < size; ++i) {
            struct TreeNode *node = dequeue(q);
            level[i] = node->val;
            if (node->left) enqueue(q, node->left);
            if (node->right) enqueue(q, node->right);
        }
        ans += calculateMinSwaps(level, size);
        free(level);
    }

    freeQueue(q);
    return ans;
}
