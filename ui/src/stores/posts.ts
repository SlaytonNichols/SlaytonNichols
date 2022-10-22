import { acceptHMRUpdate, defineStore } from "pinia"
import { ResponseStatus } from "@servicestack/client"
import { client } from "@/api"
import { PostPost, DeletePost, GetPosts, Post, PatchPost } from "@/dtos"

export const usePostsStore = defineStore('posts', () => {
    // State
    const posts = ref<Post[]>([])
    const error = ref<ResponseStatus | null>()

    // Getters
    const allPosts = computed(() => posts.value)


    // Actions
    const getPost = async (path: string) => {
        await refreshPosts()
        return posts.value.find(p => p.path === path)
    }
    const postsOrdered = async () => {
        //place favorite posts at the top, TODO: replace with tags
        let favorites = ["todos"]
        let favoritPosts = posts.value.filter(x => favorites.includes(x.path!));
        let otherPosts = posts.value.filter(x => !favorites.includes(x.path!));
        return favoritPosts.concat(otherPosts)
    }
    const refreshPosts = async (errorStatus?: ResponseStatus) => {
        error.value = errorStatus
        const api = await client.api(new GetPosts())
        if (api.succeeded) {
            posts.value = api.response?.results ?? []
            posts.value = await postsOrdered()
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
        let api = await client.api(new PostPost(post))
        await refreshPosts(api.error)
    }
    const updatePost = async (post?: Post) => {
        let postUpdate = {
            id: post?.id,
            mdText: post?.mdText,
            title: post?.title,
            path: post?.path,
            summary: post?.summary,
            draft: post?.draft
        }
        let api = await client.api(new PatchPost(postUpdate))
        await refreshPosts(api.error)
    }
    const removePost = async (id?: string) => {
        posts.value = posts.value.filter(x => x.id != id)
        let api = await client.api(new DeletePost({ id }))
        await refreshPosts(api.error)
    }
    const toggleDraftPost = async (path: string) => {
        const postUpdate = posts.value.find(p => p.path === path)
        postUpdate!.draft = postUpdate!.draft ? false : true
        let api = await client.api(new PatchPost(postUpdate))
        await refreshPosts(api.error)
    }

    return {
        error,
        allPosts,
        toggleDraftPost,
        getPost,
        refreshPosts,
        addPost,
        removePost,
        updatePost,
        postsOrdered
    }
})

if (import.meta.hot)
    import.meta.hot.accept(acceptHMRUpdate(usePostsStore, import.meta.hot))
