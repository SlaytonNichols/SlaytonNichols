import { acceptHMRUpdate, defineStore } from "pinia"
import { ResponseStatus } from "@servicestack/client"
import { client } from "@/api"
import { CreatePost, DeletePost, QueryPosts, Post, UpdatePost } from "@/dtos"

export const usePostsStore = defineStore('posts', () => {
    // State
    const posts = ref<Post[]>([])
    const error = ref<ResponseStatus | null>()

    // Getters
    const allPosts = computed(() => posts.value)


    // Actions
    const getPost = async (path: string) => {
        await refreshPosts()
        return posts.value.filter(p => p.path === path)[0]
    }
    const refreshPosts = async (errorStatus?: ResponseStatus) => {
        error.value = errorStatus
        const api = await client.api(new QueryPosts())
        if (api.succeeded) {
            posts.value = api.response!.results ?? []
        }
    }
    const addPost = async (newPost: Post) => {
        let post = {
            id: newPost?.id,
            mdText: newPost?.mdText,
            title: newPost?.title,
            path: newPost?.path,
            summary: newPost?.summary,
            draft: newPost?.draft
        }
        posts.value.push(new Post(post))
        let api = await client.api(new CreatePost(post))
        await refreshPosts(api.error)
    }
    const updatePost = async (post?: Post) => {
        // const existingPost = posts.value.find(x => x.id == id)!
        let updatedPost = {
            id: post?.id,
            mdText: post?.mdText,
            title: post?.title,
            path: post?.path,
            summary: post?.summary,
            draft: post?.draft
        }
        let api = await client.api(new UpdatePost(updatedPost))
        await refreshPosts(api.error)
    }
    const removePost = async (id?: number) => {
        posts.value = posts.value.filter(x => x.id != id)
        let api = await client.api(new DeletePost({ id }))
        await refreshPosts(api.error)
    }

    return {
        error,
        allPosts,
        getPost,
        refreshPosts,
        addPost,
        removePost,
        updatePost,
    }
})

if (import.meta.hot)
    import.meta.hot.accept(acceptHMRUpdate(usePostsStore, import.meta.hot))
